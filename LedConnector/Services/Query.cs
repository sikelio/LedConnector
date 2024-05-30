using LedConnector.Configs;
using LedConnector.Models.Database;
using LedConnector.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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

        public static async Task<bool> UpdateMessageTag(int messageId, List<string> newTags)
        {
            LedContext db = new();
            IDbContextTransaction transaction = await db.Database.BeginTransactionAsync();

            try
            {
                Message? message = await db
                    .Messages
                    .Where(m => m.Id == messageId)
                    .Include(m => m.Tags)
                    .FirstOrDefaultAsync();

                if (message == null)
                {
                    return false;
                }

                message.Tags.Clear();

                foreach (string tagName in newTags)
                {
                    Tag? tag = await db
                        .Tags
                        .Where(t => t.Name == tagName)
                        .FirstOrDefaultAsync();

                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        db.Tags.Add(tag);
                    }

                    message.Tags.Add(tag);
                }

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
            finally
            {
                await db.Database.CloseConnectionAsync();
            }
        }

        public static async Task<Message?> FindMessageById(int messageId)
        {
            LedContext db = new();
            Message? message = await db
                .Messages
                .Where(m => m.Id == messageId)
                .Include(m => m.Tags)
                .FirstOrDefaultAsync();

            await db.Database.CloseConnectionAsync();

            return message;
        }

        public static async Task<List<MessageDto>> FindAllMessages()
        {
            LedContext db = new();
            List<MessageDto> messages = await db
                .Messages
                .Include(m => m.Tags)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    RawMessage = m.RawMessage,
                    BinaryMessage = m.BinaryMessage,
                    Tags = m.Tags.Select(t => new TagDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                    }).ToList()
                })
                .ToListAsync();

            await db
                .Database
                .CloseConnectionAsync();

            return messages;
        }
    }
}
