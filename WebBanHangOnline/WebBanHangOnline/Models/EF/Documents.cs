using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace WebBanHangOnline.Models.EF
{
    public interface DocumentsPrototype
    {
        DocumentsPrototype Clone();
    }


    [Table("tb_Documents")]
    public class Documents : CommonAbstract, DocumentsPrototype
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // tự động tăng 
        public int Id { get; set; }
        public string Name { get; set; }
        public string File { get; set; }

        public override void SetCreated()
        {
            this.CreateDate = DateTime.Now;
            this.ModifierDate = DateTime.Now;
            this.CreateBy = "Admin";
            this.ModifierBy = "Admin";
        }

        public override void SetModified()
        {
            this.ModifierDate = DateTime.Now;
            this.ModifierBy = "Admin";
        }

        public override void SetCreatedBy()
        {

        }

        public override void SetModifiedBy()
        {

        }

        public DocumentsPrototype Clone()
        {
            // Khong nhan ban ID

            //Documents newDocument = new Documents();
            //newDocument.Name = Name;
            //newDocument.File = File;
            //newDocument.CreateBy = CreateBy;
            //newDocument.ModifierBy = ModifierBy;
            //newDocument.ModifierDate = ModifierDate;
            //newDocument.CreateDate = CreateDate;
            //return newDocument;

            return (DocumentsPrototype)this.MemberwiseClone();
        }
    }
}