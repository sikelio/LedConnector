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

        public static async Task<bool> AddMessage(Message message)
        {
            LedContext db = new();
            db
                .Messages
                .Add(message);

            bool success;

            try
            {
                await db
                    .SaveChangesAsync();

                success = true;
            }
            catch
            {
                success = false;
            }

            await db
                .Database
                .CloseConnectionAsync();

            return success;
        }
    }
}
