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
    using System.ComponentModel.DataAnnotations;

    public partial class HOSO_VUAN
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HOSO_VUAN()
        {
            this.CHITIET_DS = new HashSet<CHITIET_DS>();
            this.XETXUs = new HashSet<XETXU>();
        }
    
        public string MA_HoSo { get; set; }
        public string MA_NhanVien { get; set; }
        public string MA_TrangThai { get; set; }
        public string MA_LoaiVA { get; set; }
        public string Ten_VuAn { get; set; }
        public string NoiDung_VA { get; set; }
        public string Loai_HS { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> NgayNhan_HS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIET_DS> CHITIET_DS { get; set; }
        public virtual LOAI_VUAN LOAI_VUAN { get; set; }
        public virtual NHANVIEN NHANVIEN { get; set; }
        public virtual TRANGTHAI_HS TRANGTHAI_HS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<XETXU> XETXUs { get; set; }
    }
}
