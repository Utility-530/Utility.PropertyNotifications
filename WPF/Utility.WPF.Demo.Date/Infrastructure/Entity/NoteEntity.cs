using System;
using FreeSql;

namespace Utility.WPF.Demo.Date.Infrastructure.Entity {
   public class NoteEntity : BaseEntity<NoteEntity, Guid> {

      public DateTime Date { get; set; }
      public string Text { get; set; }
   }
}
