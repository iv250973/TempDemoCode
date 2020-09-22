# TempDemoCode
Пример1: Набор классов и интерфейсов слоя DAL (Data Abstraction Layer) представляющие профиль пользователя. Реализует задачу формирования объекта представляющий профиль клиента и передачу его в слой бизнес-логики (техническая реализация - ClassLibrari) передачей ссылки на интерфейс. Multiwork.LKK.DAL.DAL, Multiwork.LKK.DAL.Contact и Multiwork.LKK.Models - унаследованный код. DAL - статический класс (типичный "толстячок") реализующий как связь объектов с БД, так и поставку этих объектов (фабрика).

Пример2 - Инструмент для "распаковки" т.н. прямой URL ссылки. В веб-приложении "личный кабинет клиента" реализована возможность перехода в кабинет по ссылке вида https://{сайт}/{код} что не требует стандартной аутентикации клиента, при этом показывается минимально необходимая информация. Локация инструмента - ClassLibrary вместе с другими подобными инструментами.

Пример3 - инструмент для отображения строковых представлений для перечислений (но можно и не только для них). Локация инструмента - ClassLibrary вместе с другими подобными инструментами.
