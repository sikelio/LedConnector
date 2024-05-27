using LedConnector.Configs;
using LedConnector.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace LedConnector.Services
{
    public class Query
    {
        public static async Task<List<Message>> GetMessages()
        {
            LedContext db = new();
            List<Message> messages = await db
                .Messages
                .ToListAsync<Message>();

            await db
                .Database
                .CloseConnectionAsync();

            return messages;
        }
    }
}
