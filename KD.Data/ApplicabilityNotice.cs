//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KD.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class ApplicabilityNotice
    {
        public int Id { get; set; }
        public int IdNotice { get; set; }
        public int IdDetail { get; set; }
        public string Description { get; set; }
    
        public virtual Detail Detail { get; set; }
        public virtual Notice Notice { get; set; }
    }
}