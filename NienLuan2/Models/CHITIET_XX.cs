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
    
    public partial class CHITIET_XX
    {
        public string MA_ChiTietXX { get; set; }
        public Nullable<int> MA_XetXu { get; set; }
        public string MA_NhanVien { get; set; }
        public string MA_VaiTro { get; set; }
    
        public virtual XETXU XETXU { get; set; }
        public virtual NHANVIEN NHANVIEN { get; set; }
        public virtual VAITRO_NV VAITRO_NV { get; set; }
    }
}
