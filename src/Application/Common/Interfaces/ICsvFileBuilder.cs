using talker.Application.TodoLists.Queries.ExportTodos;
using System.Collections.Generic;

namespace talker.Application.Common.Interfaces
{
    public interface ICsvFileBuilder
    {
        byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
    }
}
