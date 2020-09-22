using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Multiwork.LKK.DAL
{
    /// <summary>
    /// Контакт-менеджер профиля
    /// </summary>
    class ProfileContactManager : IProfileContactManager
    {
        private readonly uint _idClient = 0;

        public ProfileContactManager(uint idClient)
        {
            _idClient = idClient;
        }        

        /// <summary>
        /// Получить список контактов клиента
        /// </summary>
        /// <param name="pType">Тип списка</param>
        /// <returns>Список контактов клиента</returns>
        public List<Models.Contact> GetContactList(EContactList pType)
        {
            byte TypeList = (byte)pType;
            var dalContacts = DAL.Get<Contact>(new { _idClient, TypeList });
            return dalContacts.Select(c => new Models.Contact(c)).ToList();
        }
    }

    /// <summary>
    /// Профиль-менеджер
    /// </summary>
    class ProfileManager : IProfileManager
    {
        /// <summary>
        /// Получить профиль клиента
        /// </summary>
        /// <param name="pIdClient">Ид клиента</param>
        /// <returns>Профиль</returns>
        public IStoreProfile Get(uint pIdClient)
        {
            var profile = DAL.Get<Profile>(new { pIdClient }).FirstOrDefault();
            return profile ?? DAL.GetEmpty<Profile>(new { pIdClient, this});
        }

        /// <summary>
        /// Сохранить изменения в профиле
        /// </summary>
        /// <returns>Успешно?</returns>
        public bool Save(IStoreProfile profile)
        {
            //Подготовка параметров для передачи в хранилище
            var p = new DynamicParameters();
            p.Add("@IdClient", (int)profile.IdClient, DbType.Int32, ParameterDirection.Input);
            p.Add("@Name", profile.Name, DbType.String, ParameterDirection.Input);
            p.Add("@City", profile.City, DbType.String, ParameterDirection.Input);
            p.Add("@TypeEducation", profile.TypeEducation, DbType.Byte, ParameterDirection.Input);
            p.Add("@University", profile.University, DbType.String, ParameterDirection.Input);
            p.Add("@Speciality", profile.Speciality, DbType.String, ParameterDirection.Input);
            p.Add("@FormEducation", profile.FormEducation, DbType.Byte, ParameterDirection.Input);
            p.Add("@CurrentCurse", profile.CurrentCurse, DbType.Byte, ParameterDirection.Input);
            p.Add("@EndCurse", profile.EndCurse, DbType.Byte, ParameterDirection.Input);
            //Сохранение в хранилище
            int Result = DAL.Exec(DAL.GetSP<Profile>(), p);
            return Result == 0;
        }
    }

    /// <summary>
    /// Профиль клиента
    /// </summary>
    class Profile : IStoreProfile
    {
        public Profile(uint idClient, IProfileManager profileManager, IProfileContactManager profileContactManager)
        {
            IdClient = idClient;
            //Бросаем исключения в вызывающий код, в случае отсутствия ссылок, поскольку рискуем получить неконсистентный объект 
            _profileManager = profileManager ?? throw new ArgumentException("profileManager is not must be null");
            _profileContactManager = profileContactManager ?? throw new ArgumentException("profileContactManager is not must be null");                        
        }

        /// <summary>
        /// Ид клиента
        /// </summary>
        public uint IdClient { get; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Город клиента
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Тип ВУЗа
        /// </summary>
        public byte? TypeEducation { get; set; }

        /// <summary>
        /// Наименование учебного заведения
        /// </summary>
        public string University { get; set; }

        /// <summary>
        /// Специальность клиента
        /// </summary>
        public string Speciality { get; set; }

        /// <summary>
        /// Форма обучения
        /// </summary>
        public byte? FormEducation { get; set; }

        /// <summary>
        /// Текущий курс клиента
        /// </summary>
        public byte? CurrentCurse { get; set; }

        /// <summary>
        /// Всего курсов
        /// </summary>
        public byte? EndCurse { get; set; }


        /// <summary>
        /// Список контактов
        /// </summary>
        public List<Models.Contact> Contacts
        {
            get
            {
                if(_contacts == null)
                    _contacts = _profileContactManager.GetContactList(EContactList.eConfirmed);
                return _contacts;
            }
        }

        private List<Models.Contact> _contacts = null;        

        private readonly IProfileManager _profileManager = null;

        private readonly IProfileContactManager _profileContactManager = null;
    }
}
