using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

namespace OCBT.Infra.DAL.Migrations
{
    public class OCBTContext : DbContext
    {
        public OCBTContext
         (DbContextOptions<OCBTContext> options)
         : base(options)
        { }

        [Obsolete]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //ignore
            modelBuilder.Ignore<base_context>();

            //modelBuilder.HasSequence<int>("offer_oid_seq").StartsAt(1).IncrementsBy(1);

            #region  booking_mst
            modelBuilder.Entity<booking_mst>().Property(s => s.booking_mst_xid).ForNpgsqlHasComment("序號").HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
            modelBuilder.Entity<booking_mst>().Property(s => s.order_mid).ForNpgsqlHasComment("order_mid");
            modelBuilder.Entity<booking_mst>().Property(s => s.order_oid).ForNpgsqlHasComment("order_oid");
            modelBuilder.Entity<booking_mst>().Property(s => s.prod_oid).ForNpgsqlHasComment("prod_oid");
            modelBuilder.Entity<booking_mst>().Property(s => s.go_date).ForNpgsqlHasComment("出發日期");
            modelBuilder.Entity<booking_mst>().Property(s => s.booking_model).HasDefaultValueSql("'{}'::jsonb").ForNpgsqlHasComment("訂購模組");
            modelBuilder.Entity<booking_mst>().Property(s => s.combo_model).HasDefaultValueSql("'{}'::jsonb").ForNpgsqlHasComment("商品模組");
            modelBuilder.Entity<booking_mst>().Property(s => s.booking_mst_order_status).ForNpgsqlHasComment("母單訂單狀態");
            modelBuilder.Entity<booking_mst>().Property(s => s.booking_mst_voucher_status).ForNpgsqlHasComment("母單憑證狀態");
            modelBuilder.Entity<booking_mst>().Property(s => s.voucher_deadline).ForNpgsqlHasComment("取憑證效期");
            modelBuilder.Entity<booking_mst>().Property(s => s.is_callback).ForNpgsqlHasComment("是否已callback").HasDefaultValue(false);
            modelBuilder.Entity<booking_mst>().Property(s => s.is_back).ForNpgsqlHasComment("是否已調整母單為back").HasDefaultValue(false);
            modelBuilder.Entity<booking_mst>().Property(s => s.is_need_back).ForNpgsqlHasComment("是否需要調整母單").HasDefaultValue(true);
            modelBuilder.Entity<booking_mst>().Property(s => s.monitor_start_datetime).ForNpgsqlHasComment("憑證取得起始時間");
            modelBuilder.Entity<booking_mst>().Property(s => s.create_user).ForNpgsqlHasComment("建立者").IsRequired();
            modelBuilder.Entity<booking_mst>().Property(s => s.create_datetime).ForNpgsqlHasComment("建立時間").IsRequired().HasDefaultValueSql("now()");
            modelBuilder.Entity<booking_mst>().Property(s => s.modify_user).ForNpgsqlHasComment("修改者");
            modelBuilder.Entity<booking_mst>().Property(s => s.modify_datetime).ForNpgsqlHasComment("修改時間");


            #endregion

            #region  booking_dtl
            modelBuilder.Entity<booking_dtl>().Property(s => s.booking_dtl_xid).ForNpgsqlHasComment("序號").HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn); ;
            modelBuilder.Entity<booking_dtl>().Property(s => s.booking_mst_xid).ForNpgsqlHasComment("booking_mst_xid");
            modelBuilder.Entity<booking_dtl>().Property(s => s.prod_oid).ForNpgsqlHasComment("prod_oid");
            modelBuilder.Entity<booking_dtl>().Property(s => s.package_oid).ForNpgsqlHasComment("package_oid");
            modelBuilder.Entity<booking_dtl>().Property(s => s.item_oid).ForNpgsqlHasComment("item_oid");
            modelBuilder.Entity<booking_dtl>().Property(s => s.sku_oid).HasDefaultValueSql("'{}'::jsonb").ForNpgsqlHasComment("sku_oid");
            modelBuilder.Entity<booking_dtl>().Property(s => s.real_booking_qty).ForNpgsqlHasComment("實際訂購數");
            modelBuilder.Entity<booking_dtl>().Property(s => s.voucher_file_info).HasDefaultValueSql("'[]'::jsonb").ForNpgsqlHasComment("voucher檔案");
            modelBuilder.Entity<booking_dtl>().Property(s => s.order_master_oid).ForNpgsqlHasComment("order_master_oid");
            modelBuilder.Entity<booking_dtl>().Property(s => s.order_master_mid).ForNpgsqlHasComment("order_master_mid");
            modelBuilder.Entity<booking_dtl>().Property(s => s.order_oid).ForNpgsqlHasComment("order_oid");
            modelBuilder.Entity<booking_dtl>().Property(s => s.order_mid).ForNpgsqlHasComment("order_mid");
            modelBuilder.Entity<booking_dtl>().Property(s => s.booking_dtl_order_status).ForNpgsqlHasComment("子單訂單狀態");
            modelBuilder.Entity<booking_dtl>().Property(s => s.booking_dtl_voucher_status).ForNpgsqlHasComment("子單憑證狀態");
            modelBuilder.Entity<booking_dtl>().Property(s => s.create_user).ForNpgsqlHasComment("建立者").IsRequired();
            modelBuilder.Entity<booking_dtl>().Property(s => s.create_datetime).ForNpgsqlHasComment("建立時間").IsRequired().HasDefaultValueSql("now()");
            modelBuilder.Entity<booking_dtl>().Property(s => s.modify_user).ForNpgsqlHasComment("修改者");
            modelBuilder.Entity<booking_dtl>().Property(s => s.modify_datetime).ForNpgsqlHasComment("修改時間");


            #endregion


            #region  combo_supplier

            modelBuilder.Entity<combo_supplier>().Property(s => s.combo_supplier_xid).ForNpgsqlHasComment("序號").HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn); ;
            modelBuilder.Entity<combo_supplier>().Property(s => s.supplier_oid).ForNpgsqlHasComment("供應商序號");
            modelBuilder.Entity<combo_supplier>().Property(s => s.status).ForNpgsqlHasComment("狀態");
            modelBuilder.Entity<combo_supplier>().Property(s => s.create_user).ForNpgsqlHasComment("建立者").IsRequired();
            modelBuilder.Entity<combo_supplier>().Property(s => s.create_datetime).ForNpgsqlHasComment("建立時間").IsRequired().HasDefaultValueSql("now()");
            modelBuilder.Entity<combo_supplier>().Property(s => s.modify_user).ForNpgsqlHasComment("修改者");
            modelBuilder.Entity<combo_supplier>().Property(s => s.modify_datetime).ForNpgsqlHasComment("修改時間");


            #endregion
            
            #region  booking_log
            modelBuilder.Entity<booking_log>().Property(s => s.booking_log_xid).ForNpgsqlHasComment("序號").HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn); ;
            modelBuilder.Entity<booking_log>().Property(s => s.log_type).ForNpgsqlHasComment("log類型");
            modelBuilder.Entity<booking_log>().Property(s => s.booking_mst_xid).ForNpgsqlHasComment("booking_mst_xid");
            modelBuilder.Entity<booking_log>().Property(s => s.booking_dtl_xid).ForNpgsqlHasComment("booking_dtl_xid");
            modelBuilder.Entity<booking_log>().Property(s => s.booking_source).ForNpgsqlHasComment("來源為MST/DTL");
            modelBuilder.Entity<booking_log>().Property(s => s.status_type).ForNpgsqlHasComment("類型為ORDER/VOUCHER");
            modelBuilder.Entity<booking_log>().Property(s => s.status).ForNpgsqlHasComment("狀態");
            modelBuilder.Entity<booking_log>().Property(s => s.previous_status).ForNpgsqlHasComment("上一個狀態");
            modelBuilder.Entity<booking_log>().Property(s => s.create_user).ForNpgsqlHasComment("建立者").IsRequired();
            modelBuilder.Entity<booking_log>().Property(s => s.create_datetime).ForNpgsqlHasComment("建立時間").IsRequired().HasDefaultValueSql("now()");
            modelBuilder.Entity<booking_log>().Property(s => s.modify_user).ForNpgsqlHasComment("修改者");
            modelBuilder.Entity<booking_log>().Property(s => s.modify_datetime).ForNpgsqlHasComment("修改時間");

            #endregion
        }


        public class base_context
        {
            //建立者
            [Column(TypeName = "varchar(36)", Order = 100)]
            public string create_user { get; set; }

            //建立時間
            [Column(Order = 101)]
            public DateTime create_datetime { get; set; }

            //修改者
            [Column(TypeName = "varchar(36)", Order = 102)]
            public string modify_user { get; set; }

            //修改時間
            [Column(Order = 103)]
            public DateTime? modify_datetime { get; set; }
        }

        [Table("booking_mst")]
        public class booking_mst : base_context
        {
            //序號
            [Key]
            [Column(TypeName = "int8")]
            public int booking_mst_xid { get; set; }

            //ORDER_MID
            [Column(TypeName = "varchar(13)")]
            public string order_mid { get; set; }

            //ORDER_OID
            [Column(TypeName = "int4")]
            public System.Nullable<int> order_oid { get; set; }

            //PROD_OID
            [Column(TypeName = "int4")]
            public System.Nullable<int> prod_oid { get; set; }

            //GO_DATE
            [Column(TypeName = "varchar(8)")]
            public string go_date { get; set; }

            //BOOKING_MODEL
            //java call ocbt的原始json
            [Column(TypeName = "jsonb")]
            public string booking_model { get; set; }

            //COMBO_MODEL
            //從product取得母子明細商品原始json
            [Column(TypeName = "jsonb")]
            public string combo_model { get; set; }

            //母單訂單狀態
            [Column(TypeName = "varchar(15)")]
            public string booking_mst_order_status { get; set; }

            //母單憑證狀態
            [Column(TypeName = "varchar(15)")]
            public string booking_mst_voucher_status { get; set; }

            //憑證最長等待時間
            [Column(TypeName = "int4")]
            public System.Nullable<int> voucher_deadline { get; set; }

            //是否有callback
            [Column(TypeName = "bool")]
            public System.Nullable<bool> is_callback { get; set; }

            //是否有回報母單back
            [Column(TypeName = "bool")]
            public System.Nullable<bool> is_back { get; set; }

            //是否需要回報母單back
            [Column(TypeName = "bool")]
            public System.Nullable<bool> is_need_back { get; set; }

            //monitor_start_datetime
            public DateTime? monitor_start_datetime { get; set; }
        }


        [Table("booking_dtl")]
        public class booking_dtl : base_context
        {
            //序號
            [Key]
            [Column(TypeName = "int8")]
            public int booking_dtl_xid { get; set; }

            //BOOKING_MST_XID
            [Column(TypeName = "int8")]
            public System.Nullable<int> booking_mst_xid { get; set; }

            //PROD_OID
            [Column(TypeName = "int4")]
            public System.Nullable<int> prod_oid { get; set; }

            //PACKAGE_OID
            [Column(TypeName = "int4")]
            public System.Nullable<int> package_oid { get; set; }

            //ITEM_OID
            [Column(TypeName = "int4")]
            public System.Nullable<int> item_oid { get; set; }

            //SKU_OID
            //java call ocbt的原始json
            [Column(TypeName = "jsonb")]
            public string sku_oid { get; set; }

            //實際訂購數量
            [Column(TypeName = "int4")]
            public System.Nullable<int> real_booking_qty { get; set; }

            //VOUCHER_FILE_INFO
            [Column(TypeName = "jsonb")]
            public string voucher_file_info { get; set; }

            //ORDER_MASTER_OID
            [Column(TypeName = "int4")]
            public System.Nullable<int> order_master_oid { get; set; }

            //ORDER_MASTER_MID
            [Column(TypeName = "varchar(13)")]
            public string order_master_mid { get; set; }

            //ORDER_OID
            [Column(TypeName = "int4")]
            public System.Nullable<int> order_oid { get; set; }

            //ORDER_MID
            [Column(TypeName = "varchar(13)")]
            public string order_mid { get; set; }

            //子單訂單狀態
            [Column(TypeName = "varchar(15)")]
            public string booking_dtl_order_status { get; set; }

            //子單憑證狀態
            [Column(TypeName = "varchar(15)")]
            public string booking_dtl_voucher_status { get; set; }
        }

        [Table("combo_supplier")]
        public class combo_supplier : base_context
        {
            //序號
            [Key]
            [Column(TypeName = "int8")]
            public int combo_supplier_xid { get; set; }

            //SUPPLIER_OID
            [Column(TypeName = "int4")]
            public System.Nullable<int> supplier_oid { get; set; }

            //狀態
            [Column(TypeName = "varchar(12)")]
            public string status { get; set; }

        }


        [Table("booking_log")]
        public class booking_log : base_context
        {
            //序號
            [Key]
            [Column(TypeName = "int8")]
            public int booking_log_xid { get; set; }

            //LOG_TYPE
            [Column(TypeName = "varchar(5)")]
            public string log_type { get; set; }

            //BOOKING_MST_XID
            [Column(TypeName = "int8")]
            public System.Nullable<int> booking_mst_xid { get; set; }

            //BOOKING_DTL_XID
            [Column(TypeName = "int8")]
            public System.Nullable<int> booking_dtl_xid { get; set; }

            //BOOKING_SOURCE
            [Column(TypeName = "varchar(5)")]
            public string booking_source { get; set; }

            //STATUS_TYPE
            [Column(TypeName = "varchar(15)")]
            public string status_type { get; set; }

            //STATUS
            [Column(TypeName = "varchar(20)")]
            public string status { get; set; }

            //PREVIOUS_STATUS
            [Column(TypeName = "jsonb")]
            public string previous_status { get; set; }

        }

        
    }
}
