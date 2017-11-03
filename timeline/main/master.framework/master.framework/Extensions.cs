using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace master.framework
{
    public static class Extensions
    {
        #region #region Extensions for System.Reflection.Assembly
        /// <summary>
        /// Get FileVersion of Assembly call with System.Reflection.Assembly.GetExecutingAssembly()
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string Version(this System.Reflection.Assembly assembly)
        {
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string ret = fvi.FileVersion;
            return ret;
        }
        #endregion

        #region Extensions for System.Reflection.PropertyInfo
        public static master.framework.attribute.database.Created GetCreatedAtt(this System.Reflection.PropertyInfo property)
        {
            var ret = property.GetCustomAttributes(typeof(master.framework.attribute.database.Created), true).FirstOrDefault() as master.framework.attribute.database.Created;
            return ret;

        }
        public static master.framework.attribute.database.WithHistorical GetHistoricalAtt(this System.Reflection.PropertyInfo property)
        {
            var ret = property.GetCustomAttributes(typeof(master.framework.attribute.database.WithHistorical), true).FirstOrDefault() as master.framework.attribute.database.WithHistorical;
            return ret;

        }
        public static master.framework.attribute.database.CreatedOrUpdated GetCreatedOrUpdatedAtt(this System.Reflection.PropertyInfo property)
        {
            var ret = property.GetCustomAttributes(typeof(master.framework.attribute.database.CreatedOrUpdated), true).FirstOrDefault() as master.framework.attribute.database.CreatedOrUpdated;
            return ret;

        }
        public static master.framework.attribute.database.Updated GetUpdatedAtt(this System.Reflection.PropertyInfo property)
        {
            var ret = property.GetCustomAttributes(typeof(master.framework.attribute.database.Updated), true).FirstOrDefault() as master.framework.attribute.database.Updated;
            return ret;

        }
        #endregion

        #region Extensions for System.Exception

        /// <summary>
        /// Transform Exception in <see cref="master.framework.dto.DataLogging"/>
        /// </summary>
        /// <param name="obj">Exception</param>
        /// <returns><see cref="master.framework.dto.DataLogging"/></returns>
        public static dto.DataLogging ToDataLogging(this Exception obj)
        {
            dto.DataLogging ret = new dto.DataLogging();
            var aux = obj;
            int count = 0;

            while (aux != null)
            {
                ret.Message += (string.IsNullOrWhiteSpace(ret.Message) ? "" : " ") + aux.Message;
                ret.StackTrace += count.ToString() + ": " + aux.StackTrace;
                ret.Source += count.ToString() + ": " + aux.Source;

                aux = aux.InnerException;
                count++;
            }
            return ret;
        }
        #endregion

        #region Extensions for System.Int and System.Int?
        public static string ToMonthPTBR(this int value, bool diminutive = false)
        {
            string ret = string.Empty;
            switch (value)
            {
                case 1:
                    ret = "Janeiro";
                    break;
                case 2:
                    ret = "Fevereiro";
                    break;
                case 3:
                    ret = "Março";
                    break;
                case 4:
                    ret = "Abril";
                    break;
                case 5:
                    ret = "Maio";
                    break;
                case 6:
                    ret = "Junho";
                    break;
                case 7:
                    ret = "Julho";
                    break;
                case 8:
                    ret = "Agosto";
                    break;
                case 9:
                    ret = "Setembro";
                    break;
                case 10:
                    ret = "Outubro";
                    break;
                case 11:
                    ret = "Novembro";
                    break;
                case 12:
                    ret = "Dezembro";
                    break;
            }
            if (diminutive) { ret = ret.Substring(0, 3); }
            return ret;
        }
        public static bool IsAdd(this int obj)
        {
            return obj == 0;
        }
        public static bool IsAdd(this int? obj)
        {
            return obj == 0 || !obj.HasValue;
        }
        #endregion

        #region Extensions for System.Int64 and System.Int64?
        public static bool IsAdd(this long obj)
        {
            return obj == 0;
        }
        public static bool IsAdd(this long? obj)
        {
            return obj == 0 || !obj.HasValue;
        }
        #endregion

        #region Extensions for System.DateTime and System.DateTime?
        public static int GetAge(this DateTime? bday)
        {
            if (bday.HasValue)
            {
                return bday.Value.GetAge();
            }
            else
            {
                return 0;
            }
        }
        public static int GetAge(this DateTime bday)
        {
            DateTime now = DateTime.Now;

            DateTime zeroTime = new DateTime(1, 1, 1);


            TimeSpan span = now - bday;
            // because we start at year 1 for the Gregorian 
            // calendar, we must subtract a year here.
            int years = (zeroTime + span).Year - 1;

            return years;
        }
        public static string ToString2(this DateTime? obj, bool showTime = false, bool showSeconds = false, bool onlyTime = false)
        {
            if (obj.HasValue)
            {
                return obj.Value.ToString2(showTime: showTime, showSeconds: showSeconds, onlyTime: onlyTime);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converte para string
        /// </summary>
        /// <param name="obj">objeto datetime</param>
        /// <param name="showTime">colocar hora?</param>
        /// <param name="showSeconds">colocar segundos na hora?</param>
        /// <returns>dd/MM/yyyy HH:mm:ss</returns>
        public static string ToString2(this DateTime obj, bool showTime = false, bool showSeconds = false, bool onlyTime = false)
        {
            if (obj == DateTime.MinValue) { return string.Empty; }
            string format = "dd/MM/yyyy";
            if (onlyTime)
            {
                format = "HH:mm";
                if (showSeconds)
                {
                    format += ":ss";
                }
                return obj.ToString(format);
            }
            if (showTime)
            {
                format += " HH:mm";
                if (showSeconds)
                {
                    format += ":ss";
                }
            }
            return obj.ToString(format);
        }
        #endregion

        #region Extensions for System.Object
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object to Serialize</param>
        /// <param name="maxdepth">Maximum depth of serialization</param>
        /// <returns></returns>
        public static string Serialize2(this object obj, int maxdepth = 20)
        {
            using (var strWriter = new System.IO.StringWriter())
            {
                using (var jsonWriter = new master.framework.CustomJsonTextWriter(strWriter))
                {
                    Func<bool> include = () => jsonWriter.CurrentDepth <= maxdepth;
                    var resolver = new master.framework.CustomContractResolver(include);
                    var serializer = new Newtonsoft.Json.JsonSerializer { ContractResolver = resolver };
                    serializer.Serialize(jsonWriter, obj);
                }
                return strWriter.ToString();
            }
        }
        public static string Serialize(this object obj, int maxdepth = 20)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore, MaxDepth = maxdepth, NullValueHandling = Newtonsoft.Json.NullValueHandling.Include });
        }
        #endregion

        #region Extensions for System.String
        public static bool ValidateEmail(this string value)
        {
            bool ret = false;
            if (!string.IsNullOrWhiteSpace(value))
            {
                var emails = value.Split(',');

                for (int i = 0; i < emails.Length; i++)
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(emails[i], @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        ret = false;
                    }
                    else
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }
        public static bool ValidateBrazilianRG(this string strRG, bool blankError, out List<string> msg)
        {
            bool ret = true;
            msg = new List<string>();

            if (string.IsNullOrWhiteSpace(strRG))
            {
                if (blankError)
                {
                    msg.Add("RG obrigatório.");
                    ret = false;
                }
            }
            else
            {
                if (strRG.Length > 14)
                {
                    msg.Add("RG inválido. Número de caracteres inválidos.");
                    ret = false;
                }
                else
                {
                    foreach (var item in strRG)
                    {
                        if (!Char.IsNumber(item))
                        {
                            if (!Char.IsLetter(item))
                            {
                                if (item != '.' && item != '-')
                                {
                                    msg.Add("Caractere não permitido para o campo RG.");
                                    ret = false;
                                }
                            }

                        }
                    }
                }
            }

            return ret;
        }
        public static bool ValidateBrazilianPIS(this string obj)
        {
            bool ret;

            if (obj.Trim() == "00000000000") { return false; }

            int[] multiplicador = new int[10] { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum;
            int remnant;

            if (obj.Trim().Length != 11) { return false; }

            obj = obj.Trim();
            obj = obj.Replace("-", "").Replace(".", "").PadLeft(11, '0');

            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(obj[i].ToString()) * multiplicador[i];
            }

            remnant = sum % 11;

            if (remnant < 2) { remnant = 0; }
            else { remnant = 11 - remnant; }

            ret = obj.EndsWith(remnant.ToString());

            return ret;
        }
        public static bool ValidateBrazilianCPF(this string obj)
        {
            string clearCPF;
            int[] cpfArray;
            int totalDigitI = 0;
            int totalDigitII = 0;
            int modI;
            int modII;

            clearCPF = obj.Trim();
            clearCPF = clearCPF.Replace("-", "");
            clearCPF = clearCPF.Replace(".", "");

            if (clearCPF.Length != 11) { return false; }

            if (clearCPF.Equals("00000000000") || clearCPF.Equals("11111111111") || clearCPF.Equals("22222222222") || clearCPF.Equals("33333333333") ||
             clearCPF.Equals("44444444444") || clearCPF.Equals("55555555555") || clearCPF.Equals("66666666666") || clearCPF.Equals("77777777777") ||
             clearCPF.Equals("88888888888") || clearCPF.Equals("99999999999"))
                return false;

            foreach (char c in clearCPF) { if (!char.IsNumber(c)) { return false; } }

            cpfArray = new int[11];

            for (int i = 0; i < clearCPF.Length; i++) { cpfArray[i] = int.Parse(clearCPF[i].ToString()); }

            for (int position = 0; position < cpfArray.Length - 2; position++)
            {
                totalDigitI += cpfArray[position] * (10 - position);
                totalDigitII += cpfArray[position] * (11 - position);
            }

            modI = totalDigitI % 11;
            if (modI < 2) { modI = 0; }
            else { modI = 11 - modI; }
            if (cpfArray[9] != modI) { return false; }

            totalDigitII += modI * 2;
            modII = totalDigitII % 11;

            if (modII < 2) { modII = 0; }
            else { modII = 11 - modII; }

            if (cpfArray[10] != modII) { return false; }

            return true;
        }
        public static bool ToBool(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return false; }
            var aux = text.ToBoolNull();
            return aux.HasValue ? aux.Value : false;
        }
        public static bool? ToBoolNull(this string text)
        {
            bool? ret = null;
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.Trim().ToLower();
                switch (text)
                {
                    case "1":
                    case "true":
                    case "verdadeiro":
                    case "sim":
                        ret = true;
                        break;
                    case "0":
                    case "false":
                    case "não":
                    case "nao":
                    case "falso":
                        ret = false;
                        break;
                }
            }
            return ret;
        }
        public static DateTime ToDateTime(this string text, string pattern = "dd/MM/yyyy", string cultureInfo = "pt-BR")
        {
            if (string.IsNullOrWhiteSpace(text)) { return DateTime.MinValue; }
            var aux = text.ToDateTimeNull(pattern: pattern, cultureInfo: cultureInfo);
            return aux.HasValue ? aux.Value : DateTime.MinValue;
        }
        public static DateTime? ToDateTimeNull(this string text, string pattern = "dd/MM/yyyy", string cultureInfo = "pt-BR")
        {
            DateTime? ret = null;
            if (!string.IsNullOrWhiteSpace(text))
            {
                DateTime aux = DateTime.MinValue;
                CultureInfo ci = new CultureInfo(cultureInfo);

                if (DateTime.TryParseExact(text, pattern, ci, DateTimeStyles.None, out aux))
                {
                    ret = aux;
                }
            }

            return ret;
        }
        public static int ToInt(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return 0; }
            var aux = text.ToIntNull();
            return aux.HasValue ? aux.Value : 0;
        }
        public static int? ToIntNull(this string text)
        {
            int? ret = null;
            if (!string.IsNullOrWhiteSpace(text))
            {
                int aux = 0;
                if (int.TryParse(text, out aux))
                {
                    ret = aux;
                }
            }
            return ret;
        }
        public static decimal ToDecimalNoFloatingPoint(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return 0; }
            var aux = text.ToDecimalNullNoFloatingPoint();
            return aux.HasValue ? aux.Value : 0;
        }
        public static decimal? ToDecimalNullNoFloatingPoint(this string text)
        {
            decimal? ret = null;
            if (!string.IsNullOrWhiteSpace(text))
            {
                decimal aux = 0;
                if (decimal.TryParse(text, out aux))
                {
                    ret = aux;
                }
            }
            return ret;
        }
        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static T Deserialize<T>(this string json) where T : class
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
        public static double? ToDoubleNull(this string value)
        {
            double ret = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Normalize2();
                NumberStyles style1 = NumberStyles.AllowDecimalPoint;
                NumberStyles style2 = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                CultureInfo cultureBR = CultureInfo.GetCultureInfo("pt-BR");
                CultureInfo cultureEN = CultureInfo.GetCultureInfo("en-US");
                CultureInfo cultureINV = CultureInfo.InvariantCulture;


                if (!double.TryParse(value, value.Contains(".") && value.Contains(",") ? style2 : style1, cultureBR, out ret))
                {
                    ret = 0;
                    if (!double.TryParse(value, value.Contains(".") && value.Contains(",") ? style2 : style1, cultureEN, out ret))
                    {
                        ret = 0;
                        if (!double.TryParse(value, value.Contains(".") && value.Contains(",") ? style2 : style1, cultureINV, out ret))
                        {
                            ret = 0;
                            ret = value.ToDouble2();
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return ret;
        }
        public static double ToDouble(this string value)
        {
            double ret = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Normalize2();
                NumberStyles style1 = NumberStyles.AllowDecimalPoint;
                NumberStyles style2 = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                CultureInfo cultureBR = CultureInfo.GetCultureInfo("pt-BR");
                CultureInfo cultureEN = CultureInfo.GetCultureInfo("en-US");
                CultureInfo cultureINV = CultureInfo.InvariantCulture;


                if (!double.TryParse(value, value.Contains(".") && value.Contains(",") ? style2 : style1, cultureBR, out ret))
                {
                    ret = 0;
                    if (!double.TryParse(value, value.Contains(".") && value.Contains(",") ? style2 : style1, cultureEN, out ret))
                    {
                        ret = 0;
                        if (!double.TryParse(value, value.Contains(".") && value.Contains(",") ? style2 : style1, cultureINV, out ret))
                        {
                            ret = 0;
                            ret = value.ToDouble2();
                        }
                    }
                }
            }

            return ret;
        }
        public static double ToDouble2(this string value)
        {
            double ret = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Normalize3();
                NumberStyles style = NumberStyles.AllowDecimalPoint;
                CultureInfo cultureBR = CultureInfo.GetCultureInfo("pt-BR");
                CultureInfo cultureEN = CultureInfo.GetCultureInfo("en-US");
                CultureInfo cultureINV = CultureInfo.InvariantCulture;

                if (!double.TryParse(value, style, cultureBR, out ret))
                {
                    ret = 0;
                    if (!double.TryParse(value, style, cultureEN, out ret))
                    {
                        ret = 0;
                        if (!double.TryParse(value, style, cultureINV, out ret))
                        {
                            ret = 0;
                            throw new Exception("Não foi possível converter para número.");
                        }
                    }
                }
            }

            return ret;
        }
        public static decimal ToDecimal(this string value)
        {
            decimal ret = decimal.Zero;

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Normalize2();
                NumberStyles style1 = NumberStyles.AllowDecimalPoint;
                NumberStyles style2 = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                CultureInfo cultureBR = CultureInfo.GetCultureInfo("pt-BR");
                CultureInfo cultureEN = CultureInfo.GetCultureInfo("en-US");
                CultureInfo cultureINV = CultureInfo.InvariantCulture;


                if (!decimal.TryParse(value, value.Contains(".") && value.Contains(",") ? style2 : style1, cultureBR, out ret))
                {
                    ret = decimal.Zero;
                    if (!decimal.TryParse(value, value.Contains(".") && value.Contains(",") ? style2 : style1, cultureEN, out ret))
                    {
                        ret = decimal.Zero;
                        if (!decimal.TryParse(value, value.Contains(".") && value.Contains(",") ? style2 : style1, cultureINV, out ret))
                        {
                            ret = decimal.Zero;
                            ret = value.ToDecimal2();
                        }
                    }
                }
            }

            return ret;
        }
        public static decimal ToDecimal2(this string value)
        {
            decimal ret = decimal.Zero;

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Normalize3();
                NumberStyles style = NumberStyles.AllowDecimalPoint;
                CultureInfo cultureBR = CultureInfo.GetCultureInfo("pt-BR");
                CultureInfo cultureEN = CultureInfo.GetCultureInfo("en-US");
                CultureInfo cultureINV = CultureInfo.InvariantCulture;

                if (!decimal.TryParse(value, style, cultureBR, out ret))
                {
                    ret = decimal.Zero;
                    if (!decimal.TryParse(value, style, cultureEN, out ret))
                    {
                        ret = decimal.Zero;
                        if (!decimal.TryParse(value, style, cultureINV, out ret))
                        {
                            ret = decimal.Zero;
                            throw new Exception("Não foi possível converter para número.");
                        }
                    }
                }
            }

            return ret;
        }
        public static string OnlyCharAndNumber(this string value, bool keepSpace = false)
        {
            var regex = keepSpace ? @"[^0-9a-zA-Z ]+" : @"[^0-9a-zA-Z]+";
            value = string.IsNullOrWhiteSpace(value) ? string.Empty : System.Text.RegularExpressions.Regex.Replace(value, regex, "");
            return value;
        }
        public static string Normalize2(this string value)
        {
            value = value.Replace("%", "");
            value = value.Replace("R$", "");
            value = value.Replace("$", "");
            value = value.Replace("-", "");
            value = value.Trim();
            return value;
        }
        public static string Normalize3(this string value)
        {
            value = value.Replace("%", "");
            value = value.Replace("R$", "");
            value = value.Replace("$", "");
            value = value.Replace(",", "");
            value = value.Trim();
            return value;
        }
        #endregion

        #region Extensions for System.Collections.Generic.List<System.String>
        public static string ToString2(this List<string> obj)
        {
            StringBuilder ret = new StringBuilder();
            foreach (var item in obj)
            {
                ret.AppendFormat("<br />{0}", item);
            }
            return ret.ToString();
        }
        #endregion

        #region Extensions for System.Collections.Generic.List<System.String>
        public static void Add(this Dictionary<string, string> obj, string key, string value)
        {
            if (obj.Any(o => o.Key == key))
            {
                obj[key] = value;
            }
            else
            {
                obj.Add(key, value);
            }
        }
        public static void Remove(this Dictionary<string, string> obj, string key)
        {
            if (obj.Any(o => o.Key == key))
            {
                obj.Remove(key);
            }
        }
        #endregion

        #region Extensions for System.Enum
        public static string GetDescription(this Enum value)
        {
            System.Reflection.FieldInfo field = value.GetType().GetField(value.ToString());

            System.ComponentModel.DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute))
                        as System.ComponentModel.DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }
        #endregion

        #region Extensions for System.Double and System.Double?
        public static string ToMoney(this double? value, bool zeroIfNull = false, int round = 2, string culture = "pt-br")
        {
            string ret = string.Empty;
            if (value.HasValue)
            {
                ret = value.Value.ToMoney(round, culture);
            }
            else
            {
                if (zeroIfNull) { ret = ((double)0M).ToMoney(round, culture); }
            }

            return ret;
        }
        public static string ToMoney(this double value, int round = 2, string culture = "pt-br")
        {
            string ret = string.Empty;

            ret = Math.Round(value, round).ToString("C", CultureInfo.CreateSpecificCulture(culture));
            return ret;
        }
        #endregion


    }
}
