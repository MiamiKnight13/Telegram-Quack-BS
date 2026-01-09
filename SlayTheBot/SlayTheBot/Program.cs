using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SlayTheBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Host bot = new Host(Environment.GetEnvironmentVariable("TG_BOT_QB_TOKEN"));
            bot.Start();
            Console.ReadLine();
        }
    }

    class Host
    {
        public const string OwnerId = "1369750317";
        TelegramBotClient bot;
        public Dictionary<long?, UserState> _states = new Dictionary<long?, UserState>();
        public List<Tournament> _tournaments = new List<Tournament>();

        public string tName;
        public int tMaxParticipants;
        public int tPrice;
        public int tId;

        public string UsersFilePath = "tournaments.json";

        private void LoadTournaments()
        {
            if (File.Exists(UsersFilePath))
            {
                var json = File.ReadAllText(UsersFilePath);
                var loaded = JsonSerializer.Deserialize<List<Tournament>>(json);
                _tournaments = loaded ?? new List<Tournament>();
                Console.WriteLine("tournaments have been loaded!");
            }
        }

        private void SaveTournaments()
        {
            var json = JsonSerializer.Serialize(_tournaments, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(UsersFilePath, json);
            Console.WriteLine("tournaments are saved!");
        }

        public Host(string token)
        {
            bot = new TelegramBotClient(token);
        }

        public void Start()
        {
            LoadTournaments();
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
            long? chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id;

            if (!_states.TryGetValue(chatId, out var state))
            {
                state = new UserState();
                _states[message?.Chat.Id] = state;
                Console.WriteLine($"new user: {message?.From}");
                if (chatId == 1369750317)
                {
                    state.isAdmin = true;
                }
            }

            if (message is { Text: { } text })
                {
                    Console.WriteLine($"new message from {message.From}: {text}");

                    if (text == "/start")
                    {
                    await bot.SendMessage(chatId, "БОТ НА ДАННЫЙ МОМЕНТ НЕ РАБОТАЕТ 24/7, поэтому просим пока им не пользоваться, спасибо\n\nПривет!" +
                            "\nЯ чат бот для регистрации на турниры.\nНажми 'Список активных турниров' ниже и выбери турнир, на который хочешь приобрести слот." +
                            "\nДальше я подскажу всю нужную информацию и дам все необходимые данные для участия." +
                            "\nЕсли есть тупой или умный вопрос - /support ;)"
                            , replyMarkup: new InlineKeyboardButton[][]
                        {
                    [("Правила"), ("Что такое Quack Brawl")],
                    [("Список активных турниров"), ("Поддержка")]
                        });
                    return;
                    }
                else if (text == "/help")
                {
                    if (state.isAdmin)
                    {
                        await bot.SendMessage(chatId, "Here is the guide",
                            replyMarkup: new InlineKeyboardButton("Repository Link", "https://github.com/TelegramBots/Telegram.Bot"));
                    }
                    else
                    {
                        await bot.SendMessage(chatId, "nope");
                    }
                    return;
                }
                else if(text == "/create_T" && state.isAdmin)
                {
                    await bot.SendMessage(chatId, "send the tournament name");
                    state.isWaitingForTournamentName = true;
                }
                else if(text == "/add_par" && state.isAdmin)
                {
                    await bot.SendMessage(chatId, "send the user id you want to add");
                    state.isWaitingForIdToAdd = true;
                }
                else if(text == "/del_par" && state.isAdmin)
                {
                    await bot.SendMessage(chatId, "send the tournament ID from which you want to remove a user");
                    state.isWaitingForTIdToRemoveUser = true;
                }
                else if (text == "/support")
                {
                    await bot.SendMessage(chatId, "Ваше следуйщее сообщение отправиться в поддержку...");
                    state.isWaitingForSupportMessage = true;
                }
                else if (text == "/del_T" && state.isAdmin)
                {
                    await bot.SendMessage(chatId, "send the tId you want to remove");
                    state.isWaitingForTIdToRemove = true;
                }
                else if (text == "/par_list" && state.isAdmin)
                {
                    await bot.SendMessage(chatId, "send the tId you want to check");
                    state.isWaitingForTIdToCheckList = true;
                }
                else if (text == "/playtime" && state.isAdmin)
                {
                    await bot.SendMessage(chatId, "send the tId of the tournament you want to start playing in");
                    state.isWaitingForTIdToSendMessageToAllParticipants = true;
                }
                else if (text != null && state.isWaitingForIdToAdd)
                {
                    state.isWaitingForIdToAdd = false;
                    state.IdToAdd = Convert.ToInt64(text);
                    state.isWaitingForTIDToAdd = true;
                    await bot.SendMessage(chatId, "send the tournament id");
                }
                else if (text != null && state.isWaitingForSupportMessage)
                {
                    state.isWaitingForSupportMessage = false;
                    await bot.SendMessage(OwnerId, $"new support message from {update.Message.From}:\n{text}");
                    await bot.SendMessage(chatId, $"Вы только что отрпавили '{text}' в поддержку.\n/support");
                }
                else if (text != null && state.isWaitingForTIDToAdd)
                {
                    state.isWaitingForTIDToAdd = false;
                    state.TIdToAdd = Convert.ToInt32(text);
                    foreach (var i in _tournaments)
                    {
                        if (state.TIdToAdd == i.tId)
                        {
                            if(i.availableSlots > 0)
                            {
                                i.availableSlots--;
                                i._participants.Add(state.IdToAdd);
                                await bot.SendMessage(chatId, $"new participant: {state.IdToAdd} in tournament {state.TIdToAdd}");
                                Console.WriteLine($"new participant: {state.IdToAdd} in tournament {state.TIdToAdd}");
                                SaveTournaments();
                                break;
                            }
                            else
                            {
                                await bot.SendMessage(chatId, "no more available slots for this tournamnet!");
                                break;
                            }
                        }
                    }
                }
                else if (text != null && state.isWaitingForTournamentName)
                {
                    state.isWaitingForTournamentName = false;
                    tName = text;
                    state.isWaitingForMaxParticipants = true;
                    await bot.SendMessage(chatId, "set the max participants count");
                }
                else if (text != null && state.isWaitingForTIdToCheckList)
                {
                    state.isWaitingForTIdToCheckList = false;
                    foreach (var item in _tournaments)
                    {
                        if (item.tId == Convert.ToInt32(text))
                        {
                            foreach (var i in item._participants)
                            {
                                await bot.SendMessage(chatId, Convert.ToString(i));
                            }
                            break;
                        }
                    }
                }
                else if (text != null && state.isWaitingForMaxParticipants)
                {
                    state.isWaitingForMaxParticipants = false;
                    tMaxParticipants = Convert.ToInt32(text);
                    state.isWaitingForTournamentPrice = true;
                    await bot.SendMessage(chatId, "set the tournament price");
                }
                else if (text != null && state.isWaitingForTournamentPrice)
                {
                    state.isWaitingForTournamentPrice = false;
                    tPrice = Convert.ToInt32(text);
                    state.isWaitingForTournamentId = true;
                    await bot.SendMessage(chatId, "set the tournament id");
                }
                else if (text != null && state.isWaitingForTournamentId)
                {
                    state.isWaitingForTournamentId = false;
                    tId = Convert.ToInt32(text);
                    Tournament tournament = new Tournament(tName, tMaxParticipants, tPrice, tId);
                    _tournaments.Add(tournament);
                    await bot.SendMessage(chatId, "Great! The tournament has been created.\n/create_T");
                    SaveTournaments();
                }
                else if (text != null && state.isWaitingForTIdToRemove)
                {
                    state.isWaitingForTIDToAdd = false;
                    foreach (var item in _tournaments)
                    {
                        if (item.tId == Convert.ToInt32(text))
                        {
                            _tournaments.Remove(item);
                            await bot.SendMessage(chatId, $"great! tournament {text} has been removed");
                            SaveTournaments();
                            break;
                        }
                    }
                }
                else if (text != null && state.isWaitingForTIdToSendMessageToAllParticipants)
                {
                    state.isWaitingForTIdToSendMessageToAllParticipants = false;
                    state.TIdToSendMessageToAllParticipants = Convert.ToInt32(text);
                    state.isWaitingForMessageToSendToAllParticipants = true;
                    await bot.SendMessage(chatId, "great! Now send the text you want to send to all the participants of the tournament you choose");
                }
                else if (text != null && state.isWaitingForMessageToSendToAllParticipants)
                {
                    foreach (var item in _tournaments)
                    {
                        if (item.tId == state.TIdToSendMessageToAllParticipants)
                        {
                            foreach (var item2 in item._participants)
                            {
                                await bot.SendMessage(item2, text);
                            }
                            break;
                        }
                    }
                    await bot.SendMessage(chatId, "great! Messages were sent");
                }
                else if(text != null && state.isWaitingForTIdToRemoveUser)
                {
                    state.isWaitingForTIdToRemoveUser = false;
                    state.TIdToRemoveUser = Convert.ToInt32(text);
                    state.isWaitingForUserIdToRemove = true;
                    await bot.SendMessage(chatId, "great! Now send the user id you want to remove");
                }
                else if(text != null && state.isWaitingForUserIdToRemove)
                {
                    state.isWaitingForUserIdToRemove = false;
                    foreach(var item in _tournaments)
                    {
                        if(item.tId == state.TIdToRemoveUser)
                        {
                            foreach(var item2 in item._participants)
                            {
                                if(item2 == Convert.ToInt64(text))
                                {
                                    item.availableSlots++;
                                    item._participants.Remove(item2);
                                    await bot.SendMessage(chatId, $"great! User {item2} has been removed from tournament {state.TIdToRemoveUser}");
                                    SaveTournaments();
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                else if(text != null && state.isWaitingForUserIdToConfirm)
                {
                    state.isWaitingForUserIdToConfirm = false;
                    await bot.SendMessage(chatId, "great! Now add the user. /add_par");
                    await bot.SendMessage(Convert.ToInt64(text), "Отлично! Админ @VERT1GO51 подтвердил ваше участие в турнире. Будьте доступны во время провведения игры.\nУдачи!\n/support");
                }

            }

                else if (update is { CallbackQuery: { } cbQuery })
                {
                    var messageData = cbQuery.Data;
                    Console.WriteLine(messageData);

                    if (messageData == "Правила")
                    {
                        await bot.SendMessage(chatId, "ОБЯЗАТЕЛЬНО К ПРОЧТЕНИЮ!\n Предложения и критика приветствуются - /support." +
                            "\n1. Использование багов запрещено." +
                            "\n2. Неуважительное поведение в адрес других участников, организаторов, а также оскорбления и токсичность в чатах являются основанием для исключения из турнира без возврата взноса." +
                            "\n3. Слот на турнир считается забронированным только после 100% предоплаты. Это гарантирует честный набор и ответственность участников." +
                            "\n4. Правило о неявке: Если вы не вышли на связь и не явились на турнир, взнос, к сожалению, не возвращается — ваше место мы не могли занять другим игроком.\r\nА вот важное исключение: Если жизненные обстоятельства поменялись и вы предупредили нас за 24 часа до старта — мы вернём взнос за вычетом маленькой комиссии банка. Мы сами люди и всё понимаем :)" +
                            "\n5. Если за 1 час до старта не набрано минимальное количество людей для участия, то среди зарегестрированных участников будет проводится голосование:" +
                            "\n   а) Играем неполным составом, но с меньшим призовым фондом." +
                            "\n   б) Переносим турнир на небольшой срок, пока не наберётся нужное количество участников." +
                            "\n   в) По вашему желанию, мы можем сразу перезаписать вас на другой турнир бесплатно." +
                            "\n   г) Возврат 100% взноса всем зарегистрированным участникам." +
                            "\n6. Способы оплаты взноса:" +
                            "\n   а) Перевод на карту (рекомендуется)\nВы можете перевести сумму с карты ЛЮБОГО белорусского банка на нашу карту Беларусбанка." +
                            "\n   б) Через систему ЕРИП (по номеру телефона, привязанного к нашей карте)." +
                            "\n   в) С помощью Stars через бота.", replyMarkup: new InlineKeyboardButton[][]
                        {
                    [("Правила"), ("Что такое Quack Brawl")],
                    [("Список активных турниров"), ("Поддержка")]
                        });
                    }
                    else if (messageData == "Что такое Quack Brawl")
                    {
                        await bot.SendMessage(chatId, "Мой новый проект, в который я верю!\nМоя цель — это узнать, на что я способен, и, конечно, провести классные и справедливые турниры по классной игре!\nБуду рад, если получится организовать хоть один турнир, спасибо!", replyMarkup: new InlineKeyboardButton[][]
                        {
                    [("Правила"), ("Что такое Quack Brawl")],
                    [("Список активных турниров"), ("Поддержка")]
                        });
                    }
                    else if (messageData == "Список активных турниров")
                    {
                        if (_tournaments.Count != 0)
                        {
                            foreach (var i in _tournaments)
                            {
                                await bot.SendMessage(chatId, $"{i.name}\nМаксимальное число участников: {i.maxParticipants}\nСвободных мест: {i.availableSlots}\nЦена: {i.price} Бел. руб.\nID турнира: {i.tId}", replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Зарегистрироваться", $"reg {i.tId}")));
                            }
                        }
                        else
                        {
                            await bot.SendMessage(chatId, "Извините, пока нет доступных турниров! Проверьте позже.", replyMarkup: new InlineKeyboardButton[][] { [("Правила"), ("Что такое Quack Brawl")], [("Список активных турниров"), ("Поддержка")] });
                        }
                    }
                else if (messageData == "Поддержка")
                {
                    await bot.SendMessage(chatId, "Вызови /support", replyMarkup: new InlineKeyboardButton[][] { [("Правила"), ("Что такое Quack Brawl")], [("Список активных турниров"), ("Поддержка")] });
                }
                else if(messageData.StartsWith("reg"))
                {
                    await bot.SendMessage(chatId, "Выберите способ оплаты", replyMarkup: new InlineKeyboardButton[][] {
                    [("Перевод на карту"), ("ЕРИП")],
                    [("Телеграм Звёзды")]
                        });
                }
                else if(messageData == "Перевод на карту")
                {
                    int code = new Random().Next(1000, 9999);
                    state.MessageCode = code;
                    await bot.SendMessage(chatId, $"Номер карты: 9112 3800 5670 1230 (Белорусбанк)\n\nВ комментарии к платежу укажите этот одноразовый код: {code}\n\nПосле оплаты отправьте скриншот успешного перевода денег в лс @VERT1GO51 и нажмите кнопку ниже. Дальше ожидайте подтверждения. Спасибо." 
                        , replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Оплатил и отправил скриншот!")));
                }
                else if(messageData == "ЕРИП" || messageData == "Телеграм Звёзды")
                {
                    await bot.SendMessage(chatId, "Извините, на данный момент этот способ оплаты не доступен", replyMarkup: new InlineKeyboardButton[][] {
                        [("Перевод на карту"), ("ЕРИП")],
                    [("Телеграм Звёзды")]
                        });
                }
                else if(messageData == "Оплатил и отправил скриншот!")
                {
                    await bot.SendMessage(chatId, "Спасибо! Ожидайте подтверждения админа! \nЕсть вопросы - /support ;)");
                    await bot.SendMessage(OwnerId, $"new user: {chatId} is maybe waiting for the confirmation for tournament\n message code: {state.MessageCode}", replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Confirm payment")));
                }
                else if(messageData == "Confirm payment")
                {
                    await bot.SendMessage(chatId, "send the id to confirm");
                    state.isWaitingForUserIdToConfirm = true;
                }
                }
        }
    }
    class Tournament
    {
        public string name { get; set; }
        public int maxParticipants { get; set; }
        public int availableSlots { get; set; }
        public int price {  get; set; }
        public int tId { get; set; }
        public List<long?> _participants { get; set; } = new List<long?>();

        public Tournament(string name, int maxParticipants, int price, int tId)
        {
            this.name = name;
            this.maxParticipants = maxParticipants;
            this.price = price;
            this.tId = tId;
            availableSlots = maxParticipants;
        }
    }
}
