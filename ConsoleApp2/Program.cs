// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Questa è una calcolatrice scientifica che supporta le 4 operazioni fondamentali, l'elevamento a potenza e le funzioni trigonometriche.\nSono accettate parentesi tonde per raggruppamenti di operazioni.\nDigitare HELP per l'elenco delle operazioni effettuabili e la loro sintassi.\nUna volta terminato digitare 0.\n\nOperazione : ");
        string original = Console.ReadLine().Replace(" ", "");
        while (original != "0")
        {
            if (original == "HELP")
            {
                PrintHelp();
                original = Console.ReadLine().Replace(" ", "");
            }
            string alfacarat = string.Join("", new Regex("[^\\d\\(\\)\\/\\*\\+\\-\\^\\,]").Matches(original).Select(x => x.Value));
            alfacarat = new Regex("(sin)|(tan)|(cos)").Replace(alfacarat, "");
            if (original.Length == 0 || alfacarat.Length > 0)
            {
                Console.WriteLine("Inserire valori validi e funzioni permesse\n\nOperazione : ");
                original = Console.ReadLine().Replace(" ", "");
            }
            else if (new Regex("\\(|\\)").Matches(original).Count % 2 != 0)
            {
                Console.WriteLine("Numero di parentesi non coerente\n\nOperazione : ");
                original = Console.ReadLine().Replace(" ", "");
            }
            else
            {
                Console.WriteLine($"Risultato: {Resolve(ref original, original)}\n\nOperazione : ");
                original = Console.ReadLine().Replace(" ", "");
            }
        }
    }

    private static void PrintHelp()
    {
        string helper = "\n\nElenco funzioni : \n" +
            "\n---------------------\n" +
            "SOMMA : A + B" +
            "\n---------------------\n" +
            "DIFFERENZA : A - B" +
            "\n---------------------\n" +
            "PRODOTTO : A * B" +
            "\n---------------------\n" +
            "QUOZIENTE : A / B" +
            "\n---------------------\n" +
            "POTENZA : A ^ B" +
            "\n---------------------\n" +
            "SENO : sin(A)" +
            "\n---------------------\n" +
            "COSENO : cos(A)" +
            "\n---------------------\n" +
            "TANGENTE : tan(A)" +
            "\n---------------------\n\n" +
            "Operazione : ";
        Console.WriteLine(helper);
    }

    static string Resolve(ref string original, string actual)
    {
        string or = actual;
        Regex senRgx = new Regex("sin(\\-|\\+)*\\d+(\\,\\d+)*");
        Regex cosRgx = new Regex("cos(\\-|\\+)*\\d+(\\,\\d+)*");
        Regex tanRgx = new Regex("tan(\\-|\\+)*\\d+(\\,\\d+)*");
        Regex tondeRgx = new Regex("(?<=\\(+).+(?=\\)+)");
        if (tondeRgx.Match(or).Success)
        {
            while (tondeRgx.Match(or).Success)
            {
                or = tondeRgx.Match(or).Value;
                or = Resolve(ref original, or);
            }
        }
        string operation = or.Replace("--", "+");
        operation = operation.Replace("-+", "-");
        operation = operation.Replace("+-", "-");
        try
        {
            Regex exp = new Regex("(\\-|\\+)*\\d+(\\,\\d+)*\\^(\\-|\\+)*\\d+(\\,\\d+)*");
            foreach (string el in exp.Matches(operation).Select(x => x.Value))
            {
                double A = Convert.ToDouble(el.Split("^")[0]);
                double B = Convert.ToDouble(el.Split("^")[1]);
                operation = operation.Replace(el, $"{Math.Pow(A, B)}");
            }
            Regex product = new Regex("(\\-|\\+)*\\-*\\d+(\\,\\d+)*\\*(\\-|\\+)*\\d+(\\,\\d+)*");
            foreach (string el in product.Matches(operation).Select(x => x.Value))
            {
                double A = Convert.ToDouble(el.Split("*")[0]);
                double B = Convert.ToDouble(el.Split("*")[1]);
                operation = operation.Replace(el, $"{A * B}");
            }
            Regex divide = new Regex("(\\-|\\+)*\\d+(\\,\\d+)*\\/(\\-|\\+)*\\d+(\\,\\d+)*");
            foreach (string el in divide.Matches(operation).Select(x => x.Value))
            {
                double A = Convert.ToDouble(el.Split("/")[0]);
                double B = Convert.ToDouble(el.Split("/")[1]);
                operation = operation.Replace(el, $"{A / B}");
            }
            Regex sub = new Regex("(\\-|\\+)*\\d+(\\,\\d+)*\\-(\\-|\\+)*\\d+(\\,\\d+)*");
            foreach (string el in sub.Matches(operation).Select(x => x.Value))
            {
                double A = Convert.ToDouble(el.Split("-")[0]);
                double B = Convert.ToDouble(el.Split("-")[1]);
                operation = operation.Replace(el, $"{A - B}");
            }
            Regex add = new Regex("(\\-|\\+)*\\d+(\\,\\d+)*\\+\\d+(\\,\\d+)*");
            foreach (string el in add.Matches(operation).Select(x => x.Value))
            {
                double A = Convert.ToDouble(el.Split("+")[0]);
                double B = Convert.ToDouble(el.Split("+")[1]);
                operation = operation.Replace(el, $"{A + B}");
            }
            if (original.Contains("("))
            {
                original = original.Replace($"({or})", operation);
            }
            else
            {
                original = original.Replace($"{or}", operation);
            }
            if (tondeRgx.Match(original).Success)
            {
                original = original.Replace($"({or})", operation);
                original = Resolve(ref original, original);
            }
            if (senRgx.Match(original).Success)
            {
                double amico = Convert.ToDouble(senRgx.Match(original).Value.Replace("sin", ""));
                double result = Math.Sin(amico);
                original = original.Replace($"sin{amico}", result.ToString());
                original = Resolve(ref original, original);
            }
            if (cosRgx.Match(original).Success)
            {
                double amico = Convert.ToDouble(cosRgx.Match(original).Value.Replace("cos", ""));
                double result = Math.Cos(amico);
                original = original.Replace($"{cosRgx.Match(original).Value}", result.ToString());
                original = Resolve(ref original, original);
            }
            if (tanRgx.Match(original).Success)
            {
                double amico = Convert.ToDouble(tanRgx.Match(original).Value.Replace("tan", ""));
                double result = Math.Tan(amico);
                original = original.Replace($"{tanRgx.Match(original).Value}", result.ToString());
                original = Resolve(ref original, original);
            }
            if (original == "∞")
            {
                throw new OverflowException();
            }
            return original;
        }
        catch (OverflowException ex)
        {
            return "Valore eccedente il massimo calcolabile";
        }
        catch (Exception exG)
        {
            return "Input malformato";
        }
    }
}