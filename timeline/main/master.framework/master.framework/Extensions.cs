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
