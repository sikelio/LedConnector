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

        public static async Task<Message> AddMessage(Message message)
        {
            LedContext db = new();
            db
                .Messages
                .Add(message);

            try
            {
                await db
                    .SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await db
                .Database
                .CloseConnectionAsync();

            return message;
        }

        public static async Task<bool> DeleteMessage(Message message)
        {
            LedContext db = new();
            db
                .Messages
                .Remove(message);

            bool success;

            try
            {
                await db
                    .SaveChangesAsync();

                success = true;
            }
            catch
            {
                success= false;
            }

            await db
                .Database
                .CloseConnectionAsync();

            return success;
        }

        public static async Task<bool> EditMessage(Message newMessage)
        {
            LedContext db = new();
            Message? oldMessage = db
                .Messages
                .Where(m => m.Id == newMessage.Id)
                .FirstOrDefault();

            if (oldMessage == null)
            {
                return false;
            }

            oldMessage.RawMessage = newMessage.RawMessage;
            oldMessage.BinaryMessage = newMessage.BinaryMessage;

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

        public static async Task<Tag?> FindTagByName(string name)
        {
            LedContext db = new();
            Tag? tag = db
                .Tags
                .Where(t => t.Name == name)
                .FirstOrDefault();

            await db
                .Database
                .CloseConnectionAsync();

            return tag;
        }

        public static async Task<Tag> AddTag(Tag tag)
        {
            LedContext db = new();
            db
                .Tags
                .Add(tag);

            try
            {
                await db
                    .SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await db
                .Database
                .CloseConnectionAsync();

            return tag;
        }

        public static async Task LinkTagToMessage(int messageId, int tagId)
        {
            LedContext db = new();
            db
                .MessageTags
                .Add(new()
                {
                    MessageId = messageId,
                    TagId = tagId
                });

            await db
                    .SaveChangesAsync();

            await db
                .Database
                .CloseConnectionAsync();
        }
    }
}
