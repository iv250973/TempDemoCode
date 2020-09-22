using System;
using System.Collections.Generic;
using System.Text;

namespace Multiwork.LKK.DAL
{
    /// <summary>
    /// Профиль-менеджер
    /// </summary>
    public interface IProfileManager
    {
        /// <summary>
        /// Получить профиль клиента
        /// </summary>
        /// <param name="pIdClient">Ид клиента</param>
        /// <returns>Профиль</returns>
        IStoreProfile Get(uint pIdClient);

        /// <summary>
        /// Сохранить изменения в профиле
        /// </summary>
        /// <returns>Успешно?</returns>
        bool Save(IStoreProfile profile);
    }

    /// <summary>
    /// Контакт-менеджер профиля
    /// </summary>
    public interface IProfileContactManager
    {        
        /// <summary>
        /// Получить список контактов клиента
        /// </summary>
        /// <param name="pType">Тип списка</param>
        /// <returns>Список контактов клиента</returns>
        List<Models.Contact> GetContactList(EContactList pType);
    }

    /// <summary>
    /// Хранимый профиль клиента
    /// </summary>
    public interface IStoreProfile
    {
        /// <summary>
        /// Ид клиента
        /// </summary>
        uint IdClient { get; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Город клиента
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// Тип ВУЗа
        /// </summary>
        byte? TypeEducation { get; set; }

        /// <summary>
        /// Наименование учебного заведения
        /// </summary>
        string University { get; set; }

        /// <summary>
        /// Специальность клиента
        /// </summary>
        string Speciality { get; set; }

        /// <summary>
        /// Форма обучения
        /// </summary>
        byte? FormEducation { get; set; }

        /// <summary>
        /// Текущий курс клиента
        /// </summary>
        byte? CurrentCurse { get; set; }

        /// <summary>
        /// Всего курсов
        /// </summary>
        byte? EndCurse { get; set; }

        /// <summary>
        /// Список контактов
        /// </summary>
        List<Models.Contact> Contacts { get; }
    }
}
