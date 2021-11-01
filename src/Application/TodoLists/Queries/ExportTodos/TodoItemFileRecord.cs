﻿using talker.Application.Common.Mappings;
using talker.Domain.Entities;

namespace talker.Application.TodoLists.Queries.ExportTodos
{
    public class TodoItemRecord : IMapFrom<TodoItem>
    {
        public string Title { get; set; }

        public bool Done { get; set; }
    }
}
