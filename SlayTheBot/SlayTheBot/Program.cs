using System;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SlayTheBot
{
    internal class Program
    {
        public const string TOKEN = "8186413878:AAFfvEBoHzLEwTq5B5ahVO6glVgJmdUi-q8";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Host bot = new Host(TOKEN);
            bot.Start();
            Console.ReadLine();
        }
    }

    class Host
    {
        TelegramBotClient bot;
        public Dictionary<long?, UserState> _states = new Dictionary<long?, UserState>();
        public List<Tournament> _tournaments = new List<Tournament>();

        public Host(string token)
        {
            bot = new TelegramBotClient(token);
        }

        public void Start()
        {
            bot.StartReceiving(UpdateHandler, ExcrptionHandler);
            Console.WriteLine("The bot has been started!");
            Console.ReadLine();
        }

        private async Task ExcrptionHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            Console.WriteLine(exception.Message);
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
                var message = update.Message;
                if (message is { Text: { } text })
                {
                    var chatId = update.Message?.Chat.Id;
                    if (text == "/start")
                    {
                        await bot.SendMessage(chatId, "Привет!" +
                            "\nЯ чат бот для регистрации на турниры.\nНажми 'Список активных турниров' ниже и выбери турнир, на который хочешь приобрести слот." +
                            "\nДальше я подскажу всю нужную информацию и дам все необходимые данные для участия." +
                            "\nЕсли есть тупой или умный вопрос - /support ;)"
                            , replyMarkup: new InlineKeyboardButton[][]
                        {
                    [("Правила"), ("Что такое Quack Brawl")],
                    [("Список активных турниров"), ("Поддержка")]
                        });
                    }
                    else if (text == "/help")
                    {
                        await bot.SendMessage(chatId, "Here is the guide",
                            replyMarkup: new InlineKeyboardButton("Repository Link", "https://github.com/TelegramBots/Telegram.Bot"));
                    }
                    return;
                }

                else if (update is { CallbackQuery: { } cbQuery })
                {
                    var messageData = cbQuery.Data;
                    var cbChatId = cbQuery.Message?.Chat.Id;

                    if (messageData == "Правила")
                    {
                        await bot.SendMessage(cbChatId, "ОБЯЗАТЕЛЬНО К ПРОЧТЕНИЮ!\n Предложения и критика приветствуются - /support." +
                            "\n1. Использование багов запрещено." +
                            "\n2. Неуважительное поведение в адрес других участников, организаторов, а также оскорбления и токсичность в чатах являются основанием для исключения из турнира без возврата взноса." +
                            "\n3. Слот на турнир считается забронированным только после 100% предоплаты. Это гарантирует честный набор и ответственность участников." +
                            "\n4. Правило о неявке: Если вы не вышли на связь и не явились на турнир, взнос, к сожалению, не возвращается — ваше место мы не могли занять другим игроком.\r\nА вот важное исключение: Если жизненные обстоятельства поменялись и вы предупредили нас за 24 часа до старта — мы вернём взнос за вычетом маленькой комиссии банка. Мы сами люди и всё понимаем :)" +
                            "\n5. Если за 1 час до старта не набрано минимальное количество людей для участия, то среди зарегестрированных участников будет проводится голосование:" +
                            "\n   а) Играем неполным составом, но с меньшим призовым фондом." +
                            "\n   б) Переносим турнир на небольшой срок, пока не наберётся нужное количество участников." +
                            "\n   в) По вашему желанию, мы можем сразу перезаписать вас на другой турнир бесплатно." +
                            "\n   г) Возврат 100% взноса всем зарегистрированным участникам." +
                            "\n6. Способы оплаты взноса:\n  1. Перевод на карту (рекомендуется)\r\nВы можете перевести сумму с карты ЛЮБОГО белорусского банка (Приорбанк, МТБанк, Альфа-Банк, БПС-Сбербанк и др.) на нашу карту Беларусбанка.\r\nРеквизиты: Номер карты: XXXX XXXX XXXX 1234\r\n   В комментарии укажите ваш игровой ник!\r\n\r\n2.  Через систему ЕРИП (по номеру телефона, привязанного к нашей карте).\r\n\r\n3.  С помощью Stars через бота.\r\n\r\n4.  С помощью предметов в Steam.");
                    }
                    else if (messageData == "Что такое Quack Brawl")
                    {
                        await bot.SendMessage(cbChatId, "Мой новый проект, в который я верю!\nМоя цель — это узнать, на что я способен, и, конечно, провести классные и справедливые турниры по классной игре!\nБуду рад, если получится организовать хоть один турнир, спасибо!");
                    }
                    else if (messageData == "Список активных турниров")
                    {
                        if (_tournaments.Count != 0)
                        {
                            foreach (var i in _tournaments)
                            {
                                await bot.SendMessage(cbChatId, i.name);
                            }
                        }
                        else
                        {
                            await bot.SendMessage(cbChatId, "Извините, пока нет доступных турниров! Проверьте позже.");
                        }
                    }
                    else if (messageData == "Поддержка")
                    {
                        await bot.SendMessage(cbChatId, "Вызови /support");
                    }
            }
        }
    }
    class Tournament
    {
        public string name;
        public int participantCount = 0;
        public int maxParticipants = 10;
        public int price;
        public Dictionary<string, long?> _participants = new Dictionary<string, long?>();

        public Tournament(string name, int maxParticipants, int price)
        {
            this.name = name;
            this.maxParticipants = maxParticipants;
            this.price = price;
        }
    }
}
