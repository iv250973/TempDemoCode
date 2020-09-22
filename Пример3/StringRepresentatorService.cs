using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MultiWork;

namespace BusinesLogicLayer
{

    //Engine ---------------------------------------------------------------------------------
    //Представление значений перечислений в виде строк

    /// <summary>
    /// Интерфейс класса-репрезентатора
    /// </summary>
    public interface IRepresentator
    {
        /// <summary>
        /// Получить строковое представление для некоторого переданного значения
        /// </summary>        
        /// <param name="value">значение</param>
        /// <returns></returns>
        string GetRepresentation(object value);
    }


    /// <summary>
    /// Прейскурант репрезентаторов
    /// </summary>
    public class SetOfRepresentators
    {
        private readonly Dictionary<Type, Type> list = new Dictionary<Type, Type>();

        /// <summary>
        /// Добавить репрезентатор в набор
        /// </summary>
        /// <param name="representatorType"></param>
        /// <param name="providedType"></param>
        public void Add(Type representatorType, Type providedType)
        {
            if (RepresentatorExists(representatorType))
                throw new  ArgumentException("Репрезентатор " + representatorType.Name + " уже существует в наборе");

            if (ProvidedExists(providedType))
                throw new ArgumentException("Для типа " + providedType.Name + " уже добавлен репрезентатор");

            if (representatorType.GetInterface(typeof(IRepresentator).Name) == typeof(IRepresentator))
                list.Add(representatorType, providedType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providedType"></param>
        /// <returns></returns>
        public Type Get(Type providedType)
        {
            foreach(KeyValuePair<Type, Type> pair in list)
            {
                if (pair.Value == providedType)
                    return pair.Key;
            }
            return null;
        }

        private bool RepresentatorExists(Type representatorType)
        {
            foreach (KeyValuePair<Type, Type> pair in list)
            {
                if (pair.Key == representatorType)
                    return true;
            }
            return false;
        }

        private bool ProvidedExists(Type providedType)
        {
            foreach (KeyValuePair<Type, Type> pair in list)
            {
                if (pair.Value == providedType)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Фабрика репрезентаторов
    /// </summary>
    public static class RepresentatorsFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="providedType"></param>
        /// <param name="workSet"></param>
        /// <returns></returns>
        public static IRepresentator GetRepresentator(Type providedType, SetOfRepresentators workSet)
        {
            Type reprType = workSet.Get(providedType);
            if (reprType != null)
                return (IRepresentator)Activator.CreateInstance(reprType);
            else
                throw new ArgumentException("Для типа " + providedType.Name + "не найден соответствующий репрезентатор");                
        }        
    }


    //Engine using ------------------------------------------------------------------------------

    /// <summary>
    /// Репрезентатор уровня автора
    /// </summary>
    class EnLevelAuthorRepresentator : IRepresentator
    {
        public string GetRepresentation(object value)
        {
            return dList[(EnLevelAuthor)value];
        }
        
        private static readonly Dictionary<EnLevelAuthor, string> dList = new Dictionary<EnLevelAuthor, string>
        {
            { EnLevelAuthor.eNone, "Не определена!!!"},
            { EnLevelAuthor.ePotential, "Потенциальные" },
            { EnLevelAuthor.eFire, "Огонь" },
            { EnLevelAuthor.eFireWater, "Огонь, вода" },
            { EnLevelAuthor.eFireWaterCuprumTube, "Огонь, вода и медные трубы" },
            { EnLevelAuthor.eNotActual, "Неактуальные субчики" },
            { EnLevelAuthor.eFWCDemo, "Огонь, вода и медные трубы (демо)" }
        };
    }

    /// <summary>
    /// Репрезентатор типа учётки автора
    /// </summary>
    class ETypeAuthorAccountRepresentator : IRepresentator
    {
        public string GetRepresentation(object value)
        {
            return dList[(ETypeAuthorAccount)value];
        }
        
        private static readonly Dictionary<ETypeAuthorAccount, string> dList = new Dictionary<ETypeAuthorAccount, string>
        {
            { ETypeAuthorAccount.Agreed, "Полупрофессиональный"},
            { ETypeAuthorAccount.Professional, "Профессиональный" },
            { ETypeAuthorAccount.Restricted, "Базовый" },
        };
    }

    /// <summary>
    /// Репрезентатор юридического статуса
    /// </summary>
    class EnUrStatusRepresentator : IRepresentator
    {
        public string GetRepresentation(object value)
        {
            return dList[(EnUrStatus)value];
        }
        
        private static readonly Dictionary<EnUrStatus, string> dList = new Dictionary<EnUrStatus, string>
        {
            { EnUrStatus.eForeigner, "Иностранец"},
            { EnUrStatus.eIndividualCommers, "Индивидуальный предприниматель" },
            { EnUrStatus.ePhisical, "Физическое лицо" },
            { EnUrStatus.eSelfEmployer, "Самозанятый" },
            { EnUrStatus.eUnknown, "Неизвестно" },
        };
    }

    /// <summary>
    /// Репрезентатор статуса чека
    /// </summary>
    class EnReceiptStatusRepresentator : IRepresentator
    {
        public string GetRepresentation(object value)
        {
            return dList[(EReceiptStatus)value];
        }

        private static readonly Dictionary<EReceiptStatus, string> dList = new Dictionary<EReceiptStatus, string>
        {
            { EReceiptStatus.Accepted, "Принят"},
            { EReceiptStatus.Rejected, "Отклонён" },
            { EReceiptStatus.OnCheck, "На проверке" },
            { 0, "Отсутствует" },
        };
    }

    /// <summary>
    /// Репрезентатор статуса персональных данных
    /// </summary>
    class EnPersonalDataStateRepresentator : IRepresentator
    {
        public string GetRepresentation(object value)
        {
            return dList[(EPersonalDataState)value];
        }

        private static readonly Dictionary<EPersonalDataState, string> dList = new Dictionary<EPersonalDataState, string>
        {
            { EPersonalDataState.IsNew, "Новые"},
            { EPersonalDataState.Accepted, "Одобрены" },
            { EPersonalDataState.Rejected, "Не одобрены" },            
        };
    }

    /// <summary>
    /// Набор стандартных репрезентаторов
    /// </summary>
    public static class MultiWorkTypesStandartRepresentator
    {
        /// <summary>
        /// Получить строковое представление
        /// </summary>
        /// <param name="providedType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetRepresentation(Type providedType, object value)
        {
            try
            {
                return RepresentatorsFactory.GetRepresentator(providedType, WorkSet).GetRepresentation(value);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Получить строковое представление
        /// </summary>        
        /// <param name="value">Значение</param>
        /// <returns></returns>
        public static string GetRepresentation(object value)
        {
            try
            {
                return RepresentatorsFactory.GetRepresentator(value.GetType(), WorkSet).GetRepresentation(value);
            }
            catch
            {
                return "";
            }
        }

        private static SetOfRepresentators WorkSet
        {
            get
            {
                if (_workSet == null)
                {
                    SetOfRepresentators _workSet = new SetOfRepresentators();
                    _workSet.Add(typeof(EnLevelAuthorRepresentator), typeof(enLevelAuthor));
                    _workSet.Add(typeof(ETypeAuthorAccountRepresentator), typeof(eTypeAuthorAccount));
                    _workSet.Add(typeof(EnUrStatusRepresentator), typeof(enUrStatus));
                    _workSet.Add(typeof(EnReceiptStatusRepresentator), typeof(eReceiptStatus));
                    _workSet.Add(typeof(EnPersonalDataStateRepresentator), typeof(ePersonalDataState));
                }
                return _workSet;
            }
        }

        private static SetOfRepresentators _workSet = null;
    }
}
