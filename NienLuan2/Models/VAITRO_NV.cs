//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NienLuan2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VAITRO_NV
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VAITRO_NV()
        {
            this.CHITIET_XX = new HashSet<CHITIET_XX>();
        }
    
        public string MA_VaiTro { get; set; }
        public string Ten_VT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIET_XX> CHITIET_XX { get; set; }
    }
}
