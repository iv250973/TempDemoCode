using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Multiwork.LKK.Models
{
    /// <summary>
    /// Интерфейс распаковщика ссылок
    /// </summary>
    public interface IDirectLinkUnpacker
    {
        /// <summary>
        /// Распаковать прямую ссылку
        /// </summary>
        /// <param name="linkCode">код ссылки</param>
        /// <returns></returns>
        IDirectLinkContext GetDirectLinkContext(string linkCode, bool loadLinkedObject = false);
    }

    /// <summary>
    /// Интерфейс проверятора ссылок
    /// </summary>
    internal interface IDirectLinkChecker
    {
        /// <summary>
        /// Проверка линк-кода на корректность
        /// </summary>
        /// <param name="linkCode">линк-код</param>
        /// <returns></returns>
        bool CheckLinkCode(string linkCode);
    }   

    /// <summary>
    /// Базовый класс проверятора ссылок
    /// </summary>
    internal abstract class DirectLinkChecker : IDirectLinkChecker
    {
        /// <summary>
        /// Регулярка проверки формата кода прямой ссылки
        /// </summary>
        protected virtual Regex RegDirLink
        {
            get { return new Regex("^[A-F0-9]{32}$", RegexOptions.Compiled); }
        }

        /// <summary>
        /// Проверка линк-кода на корректность
        /// </summary>
        /// <param name="linkCode">линк-код</param>
        /// <returns></returns>
        public virtual bool CheckLinkCode(string linkCode)
        {
            if (string.IsNullOrEmpty(linkCode))
            {
                return false;
            }
            else
            {
                return RegDirLink.IsMatch(linkCode);
            }
        }
    }


    internal class DirectLinkChecker_Order : DirectLinkChecker
    {        
        public override bool CheckLinkCode(string linkCode)
        {
            return base.CheckLinkCode(linkCode);
        }
    }


    internal class DirectLinkChecker_Invoice : DirectLinkChecker
    {
        public override bool CheckLinkCode(string linkCode)
        {
            Guid g = Guid.NewGuid();
            if (!Guid.TryParse(linkCode, out g))
            {
                return false;
            }
            else
            {
                return base.CheckLinkCode(linkCode);
            }
        }
    }

    /// <summary>
    /// Распаковщик прямой ссылки (базовый)
    /// </summary>
    internal abstract class DirectLinkUnpacker : IDirectLinkUnpacker
    {
        /// <summary>
        /// Получить проверятор ссылки
        /// </summary>
        /// <returns></returns>
        protected abstract IDirectLinkChecker GetChecker();

        /// <summary>
        /// Получить контекст прямой ссылки
        /// </summary>
        /// <param name="linkCode">код ссылки</param>
        /// <returns></returns>
        public abstract IDirectLinkContext GetDirectLinkContext(string linkCode, bool loadLinkedObject = false);
    }

    /// <summary>
    /// Распаковщик для ссылки на счёт
    /// </summary>
    public class DirectLinkUnpacker_Invoice : DirectLinkUnpacker
    {
        protected override IDirectLinkChecker GetChecker()
        {
            return new DirectLinkChecker_Invoice();
        }

        public override IDirectLinkContext GetDirectLinkContext(string linkCode, bool loadLinkedObject = false)
        {
            if (!GetChecker().CheckLinkCode(linkCode))
            {
                return new DirectLinkInvoiceContext(linkCode) { Status = eLinkOrderStatus.Invalid };
            }

            //Получаем объект - счёт
            var inv = Invoice.Get(Guid.Parse(linkCode));

            if (inv == null)
            {
                return new DirectLinkInvoiceContext(linkCode) { Status = eLinkOrderStatus.NotExist };
            }
            else
            {
                return new DirectLinkInvoiceContext(linkCode)
                    {
                        Status = eLinkOrderStatus.Valid,
                        Client = inv.Client,
                        Invoice = loadLinkedObject ? inv : null,
                        IsRegistered = inv.Client.HasPassword
                    };
            }
        }
    }

    /// <summary>
    /// Распаковщик для ссылки на заказ
    /// </summary>
    public class DirectLinkUnpacker_Order : DirectLinkUnpacker
    {
        protected override IDirectLinkChecker GetChecker()
        {
            return new DirectLinkChecker_Order();
        }

        public override IDirectLinkContext GetDirectLinkContext(string linkCode, bool loadLinkedObject = false)
        {
            if (!GetChecker().CheckLinkCode(linkCode))
                return new DirectLinkOrderContext(linkCode) { Status = eLinkOrderStatus.Invalid };

            //Получаем описатель заказа
            var p = DAL.Client.DirectLinkUnPack(linkCode);

            if (p.Key == -1 && p.Value == -1)
            {
                return new DirectLinkOrderContext(linkCode) { Status = eLinkOrderStatus.NotExist };
            }
            else if (p.Key == -2)
            {
                return new DirectLinkOrderContext(linkCode) { Status = eLinkOrderStatus.Expired, IsRegistered = p.Value == 1 };
            }
            else
            {
                //Получаем клиента по описателю заказа
                var client = new Client((uint)p.Key);
                return new DirectLinkOrderContext(linkCode)
                    {
                        Status = eLinkOrderStatus.Valid,
                        Client = client,
                        Order = loadLinkedObject ? new Order(client, (uint)p.Value) : null,
                        IsRegistered = client.HasPassword
                    };
            }
        }
    }

    /// <summary>
    /// Распаковщик прямых ссылок
    /// </summary>
    public static class DirectLinkServiceManager
    {
        /// <summary>
        /// Получить контекст прямой ссылки
        /// </summary>
        /// <typeparam name="T">Класс-представление развёрнутой прямой ссылки</typeparam>
        /// <param name="linkCode">код ссылки</param>
        /// <param name="loadLinkedObject">линкованный объект</param>
        /// <returns></returns>
        public static IDirectLinkContext GetDirectLinkContext<T>(string url, LinkPageState lpState) where T : IDirectLinkUnpacker, new()
        {            
            return (new T()).GetDirectLinkContext(UrlParser.GetLinkCode(url), lpState.IsNeedForViewing());
        }

        //Здесь из демонстрации, ради читабельности примера, убран код который работал с унаследованными классами и объектами реализованными в других модулях.
    }
        
}
