using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace master.framework
{
    public static class Extensions
    {
        #region Extensions for System.Reflection.PropertyInfo
        public static master.framework.attribute.database.Created GetCreatedAtt(this System.Reflection.PropertyInfo property)
        {
            var ret = property.GetCustomAttributes(typeof(master.framework.attribute.database.Created), true).FirstOrDefault() as master.framework.attribute.database.Created;
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
                ret.Message += count.ToString() + ": " + aux.Message;
                ret.StackTrace += count.ToString() + ": " + aux.StackTrace;
                ret.Source += count.ToString() + ": " + aux.Source;

                aux = aux.InnerException;
                count++;
            }
            return ret;
        }
        #endregion

        #region Extensions for System.Int and System.Int?
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
    }
}
