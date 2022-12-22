using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        PrintHelp();
        Console.WriteLine("Sono accettate parentesi tonde per raggruppamenti di operazioni.\nDigitare HELP per l'elenco delle operazioni effettuabili e la loro sintassi.\nUna volta terminato digitare 0.\n\nOperazione : ");
        string? original = Console.ReadLine();
        while (original != null && original.Replace(" ", "") != "0")
        {
            original = original.Replace(" ", "");
            if (original == "HELP")
            {
                PrintHelp();
                Console.WriteLine("Operazione : ");
                original = Console.ReadLine();
            }
            else if (original != null)
            {
                string alfacarat = string.Join("", new Regex("[^\\d\\(\\)\\/\\*\\+\\-\\^\\,]").Matches(original).Select(x => x.Value));
                alfacarat = new Regex("(sin)|(tan)|(cos)|(ln)").Replace(alfacarat, "");
                if (original.Length == 0 || alfacarat.Length > 0 || !new Regex("\\d").Match(original).Success)
                {
                    Console.WriteLine("Inserire valori validi e/o funzioni permesse\n\nOperazione : ");
                    original = Console.ReadLine();
                }
                else if (new Regex("\\(|\\)").Matches(original).Count % 2 != 0)
                {
                    Console.WriteLine("Numero di parentesi non coerente\n\nOperazione : ");
                    original = Console.ReadLine();
                }
                else
                {
                    try
                    {
                        double result = Convert.ToDouble(Resolve(ref original, original));
                        Console.WriteLine($"Risultato: {result}\n\nOperazione : ");
                    }
                    catch
                    {
                        Console.WriteLine("Incorsi errori nel calcolo. Verificare di aver inserito i vari simboli correttamente.\n\nOperazione : ");
                    }
                    original = Console.ReadLine();
                }
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
            "\n---------------------\n" +
            "LOGARITMO NATURALE : ln(A)" +
            "\n---------------------\n\n";
            Console.WriteLine(helper);
    }

    private static Regex senRgx = new Regex("sin(\\-|\\+)*\\d+(\\,\\d+)*");
    private static Regex cosRgx = new Regex("cos(\\-|\\+)*\\d+(\\,\\d+)*");
    private static Regex tanRgx = new Regex("tan(\\-|\\+)*\\d+(\\,\\d+)*");
    private static Regex LnRgx = new Regex("ln(\\-|\\+)*\\d+(\\,\\d+)*");
    private static Regex tondeRgx = new Regex("\\([^\\)\\(]+\\)");


    static string Resolve(ref string original, string actual)
    {
        string or = actual;
        while (tondeRgx.Match(or).Success && tondeRgx.Match(or).Value != or)
        {
            or = tondeRgx.Match(or).Value;
            or = Resolve(ref original, or);
        }
        if (or.Contains("ecce") || or == "")
        {
            return "";
        }
        string operation = or.Replace("--", "+");
        operation = operation.Replace("-+", "-");
        operation = operation.Replace("+-", "-");
        if (or.StartsWith("(") && or.EndsWith(")"))
        {
            or = or.Replace("(", "");
            or = or.Replace(")", "");
        }
        if (operation.StartsWith("(") && operation.EndsWith(")"))
        {
            operation = operation.Replace("(", "");
            operation = operation.Replace(")", "");
        }
        try
        {
            Regex exp = new Regex("(\\-)*\\d+(\\,\\d+)*\\^(\\-)*\\d+(\\,\\d+)*");
            while (exp.IsMatch(operation))
            {
                string el = exp.Match(operation).Value;
                double A = Convert.ToDouble(el.Split("^")[0]);
                double B = Convert.ToDouble(el.Split("^")[1]);
                double qua = Math.Pow(A, B);
                operation = operation.Replace(el, qua>0&&A<0 ? $"+{qua}" : $"{qua}");
            }
            Regex product = new Regex("(\\-)*\\-*\\d+(\\,\\d+)*\\*(\\-)*\\d+(\\,\\d+)*");
            while (product.IsMatch(operation))
            {
                string el = product.Match(operation).Value;
                double A = Convert.ToDouble(el.Split("*")[0]);
                double B = Convert.ToDouble(el.Split("*")[1]);
                double qua = A*B;
                operation = operation.Replace(el, qua > 0 && A < 0 ? $"+{qua}" : $"{qua}");
            }
            Regex divide = new Regex("(\\-)*\\d+(\\,\\d+)*\\/(\\-)*\\d+(\\,\\d+)*");
            while (divide.IsMatch(operation))
            {
                string el = divide.Match(operation).Value;
                double A = Convert.ToDouble(el.Split("/")[0]);
                double B = Convert.ToDouble(el.Split("/")[1]);
                double qua = A / B;
                operation = operation.Replace(el, qua > 0 && A < 0 ? $"+{qua}" : $"{qua}");
            }
            Regex sub = new Regex("(\\-)*\\d+(\\,\\d+)*\\-(\\-)*\\d+(\\,\\d+)*");
            while (sub.IsMatch(operation))
            {
                string el = sub.Match(operation).Value;
                if (el[0].ToString() != "-")
                {
                    double A = Convert.ToDouble(el.Split("-")[0]);
                    double B = Convert.ToDouble(el.Split("-")[1]);
                    operation = operation.Replace(el, $"{A - B}");
                }
                else
                {
                    string[] sub2 = new Regex("(\\-)*\\d+(\\,\\d+)*").Matches(el).Select(t=> t.Value).ToArray();
                    double A = Convert.ToDouble(sub2[0]);
                    double B = Convert.ToDouble(sub2[1]);
                    operation = operation.Replace(el, $"{A - B}");
                }
            }
            Regex add = new Regex("(\\-)*\\d+(\\,\\d+)*\\+\\d+(\\,\\d+)*");
            while (add.IsMatch(operation))
            {
                string el = add.Match(operation).Value;
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
            if (senRgx.Match(original).Success)
            {
                double amico = Convert.ToDouble(senRgx.Match(original).Value.Replace("sin", ""));
                double result = Math.Sin(Math.PI / 180 * amico);
                original = original.Replace($"sin{amico}", result.ToString());
                original = Resolve(ref original, original);
            }
            if (cosRgx.Match(original).Success)
            {
                double amico = Convert.ToDouble(cosRgx.Match(original).Value.Replace("cos", ""));
                double result = Math.Cos(Math.PI / 180 * amico);
                original = original.Replace($"{cosRgx.Match(original).Value}", result.ToString());
                original = Resolve(ref original, original);
            }
            if (tanRgx.Match(original).Success)
            {
                double amico = Convert.ToDouble(tanRgx.Match(original).Value.Replace("tan", ""));
                double result = Math.Tan(Math.PI / 180 * amico);
                original = original.Replace($"{tanRgx.Match(original).Value}", result.ToString());
                original = Resolve(ref original, original);
            }
            if (LnRgx.Match(original).Success)
            {
                double amico = Convert.ToDouble(LnRgx.Match(original).Value.Replace("ln", ""));
                double result = Math.Log(amico);
                original = original.Replace($"{LnRgx.Match(original).Value}", result.ToString());
                original = Resolve(ref original, original);
            }
            if (tondeRgx.Match(original).Success)
            {
                original = original.Replace($"({or})", operation);
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
            return $"Valore eccedente il massimo calcolabile: {ex.Message}";
        }
        catch (Exception exG)
        {
            return $"Input malformato. Generata eccezione: {exG.Message}";
        }
    }
}