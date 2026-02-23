using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


var botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_TOKEN"));
long adminChatId = 6168923159;

var userStates = new Dictionary<long, string>();
var userData = new Dictionary<long, string>();

botClient.StartReceiving(UpdateHandler, ErrorHandler);
await Task.Delay(-1);
Console.ReadLine();

async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken ct)
{
    if (update.Message is not { } message) return;
    long chatId = message.Chat.Id;
    if (message.Text == "/start")
    {
        var menu = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "Diplom ishi 🎓", "Kurs ishi 📚" }
        })
        { ResizeKeyboard = true };

        await bot.SendTextMessageAsync(chatId, "Assalomu alaykum! Ish turini tanlang:", replyMarkup: menu);
        userStates.Remove(chatId);
        return;
    }
    if (message.Text == "Diplom ishi 🎓" || message.Text == "Kurs ishi 📚")
    {
        userStates[chatId] = "WAITING_TOPIC";
        userData[chatId] = $"Tur: {message.Text}";
        await bot.SendTextMessageAsync(chatId, "📝 MAVZU: \nIltimos, ish mavzusini to'liq kiriting:");
        return;
    }

    if (userStates.ContainsKey(chatId) && userStates[chatId] == "WAITING_TOPIC")
    {
        userData[chatId] += $" | Mavzu: {message.Text}";
        userStates[chatId] = "WAITING_CONTACT";

        await bot.SendTextMessageAsync(chatId, "📱 BOG'LANISH: \nSiz bilan bog'lanishimiz uchun Telegram lichkangizni (@username) yoki telefon raqamingizni yuboring:");
        return;
    }
    if (userStates.ContainsKey(chatId) && userStates[chatId] == "WAITING_CONTACT" && message.Type == MessageType.Text)
    {
        userData[chatId] += $" | Lichka: {message.Text}";
        userStates[chatId] = "WAITING_CHECK";

        string paymentText = "💰 To'lov summasi: 50 000 so'm\n\n" +
                             "💳 Karta raqami: `8600 0000 0000 0000` \n" +
                             "(Karta raqami ustiga bossangiz nusxa olinadi)\n\n" +
                             "To'lovni amalga oshirib, chekni rasm ko'rinishida yuboring. 📸";

        await bot.SendTextMessageAsync(chatId, paymentText, parseMode: ParseMode.Markdown);
        return;
    }

    if (message.Type == MessageType.Photo && userStates.ContainsKey(chatId) && userStates[chatId] == "WAITING_CHECK")
    {

        string adminInfo = $"🔔 YANGI BUYURTMA!\n\n" +
                           $"👤 Kimdan: @{message.From.Username ?? "Noma'lum"}\n" +
                           $"🆔 ID: {message.From.Id}\n" +
                           $"📝 Tafsilotlar: {userData[chatId]}";

        await bot.SendTextMessageAsync(adminChatId, adminInfo);
        await bot.ForwardMessageAsync(adminChatId, chatId, message.MessageId);

        await bot.SendTextMessageAsync(chatId, "✅ Rahmat! Ma'lumotlar va chek qabul qilindi. Tez orada operatorimiz siz bilan bog'lanadi.");

        userStates.Remove(chatId);
        userData.Remove(chatId);
        return;
    }
}

async Task ErrorHandler(ITelegramBotClient bot, Exception ex, CancellationToken ct)
{
    Console.WriteLine("Xatolik: " + ex.Message);
}