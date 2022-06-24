﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OCBT.Infra.DAL.Migrations;

namespace KKday.Web.OCBT.Migrations
{
    [DbContext(typeof(OCBTContext))]
    [Migration("20220623015510_InitialCreate_01")]
    partial class InitialCreate_01
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("OCBT.Infra.DAL.Migrations.OCBTContext+booking_dtl", b =>
                {
                    b.Property<long>("booking_dtl_xid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int8")
                        .HasComment("序號")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("booking_dtl_order_status")
                        .HasColumnType("varchar(15)")
                        .HasComment("子單訂單狀態");

                    b.Property<string>("booking_dtl_voucher_status")
                        .HasColumnType("varchar(15)")
                        .HasComment("子單憑證狀態");

                    b.Property<long?>("booking_mst_xid")
                        .HasColumnType("int8")
                        .HasComment("booking_mst_xid");

                    b.Property<DateTime>("create_datetime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("now()")
                        .HasComment("建立時間");

                    b.Property<string>("create_user")
                        .IsRequired()
                        .HasColumnType("varchar(36)")
                        .HasComment("建立者");

                    b.Property<int?>("item_oid")
                        .HasColumnType("int4")
                        .HasComment("item_oid");

                    b.Property<DateTime?>("modify_datetime")
                        .HasColumnType("timestamp without time zone")
                        .HasComment("修改時間");

                    b.Property<string>("modify_user")
                        .HasColumnType("varchar(36)")
                        .HasComment("修改者");

                    b.Property<string>("order_master_mid")
                        .HasColumnType("varchar(13)")
                        .HasComment("order_master_mid");

                    b.Property<int?>("order_master_oid")
                        .HasColumnType("int4")
                        .HasComment("order_master_oid");

                    b.Property<string>("order_mid")
                        .HasColumnType("varchar(13)")
                        .HasComment("order_mid");

                    b.Property<int?>("order_oid")
                        .HasColumnType("int4")
                        .HasComment("order_oid");

                    b.Property<int?>("package_oid")
                        .HasColumnType("int4")
                        .HasComment("package_oid");

                    b.Property<int?>("prod_oid")
                        .HasColumnType("int4")
                        .HasComment("prod_oid");

                    b.Property<int?>("real_booking_qty")
                        .HasColumnType("int4")
                        .HasComment("實際訂購數");

                    b.Property<string>("sku_oid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("jsonb")
                        .HasDefaultValueSql("'{}'::jsonb")
                        .HasComment("sku_oid");

                    b.Property<string>("voucher_file_info")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("jsonb")
                        .HasDefaultValueSql("'[]'::jsonb")
                        .HasComment("voucher檔案");

                    b.HasKey("booking_dtl_xid");

                    b.ToTable("booking_dtl");
                });

            modelBuilder.Entity("OCBT.Infra.DAL.Migrations.OCBTContext+booking_log", b =>
                {
                    b.Property<long>("booking_log_xid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int8")
                        .HasComment("序號")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<long?>("booking_dtl_xid")
                        .HasColumnType("int8")
                        .HasComment("booking_dtl_xid");

                    b.Property<long?>("booking_mst_xid")
                        .HasColumnType("int8")
                        .HasComment("booking_mst_xid");

                    b.Property<string>("booking_source")
                        .HasColumnType("varchar(5)")
                        .HasComment("來源為MST/DTL");

                    b.Property<DateTime>("create_datetime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("now()")
                        .HasComment("建立時間");

                    b.Property<string>("create_user")
                        .IsRequired()
                        .HasColumnType("varchar(36)")
                        .HasComment("建立者");

                    b.Property<string>("log_type")
                        .HasColumnType("varchar(5)")
                        .HasComment("log類型");

                    b.Property<DateTime?>("modify_datetime")
                        .HasColumnType("timestamp without time zone")
                        .HasComment("修改時間");

                    b.Property<string>("modify_user")
                        .HasColumnType("varchar(36)")
                        .HasComment("修改者");

                    b.Property<string>("previous_status")
                        .HasColumnType("jsonb")
                        .HasComment("上一個狀態");

                    b.Property<string>("status")
                        .HasColumnType("varchar(20)")
                        .HasComment("狀態");

                    b.Property<string>("status_type")
                        .HasColumnType("varchar(15)")
                        .HasComment("類型為ORDER/VOUCHER");

                    b.HasKey("booking_log_xid");

                    b.ToTable("booking_log");
                });

            modelBuilder.Entity("OCBT.Infra.DAL.Migrations.OCBTContext+booking_mst", b =>
                {
                    b.Property<long>("booking_mst_xid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int8")
                        .HasComment("序號")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("booking_model")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("jsonb")
                        .HasDefaultValueSql("'{}'::jsonb")
                        .HasComment("訂購模組");

                    b.Property<string>("booking_mst_order_status")
                        .HasColumnType("varchar(15)")
                        .HasComment("母單訂單狀態");

                    b.Property<string>("booking_mst_voucher_status")
                        .HasColumnType("varchar(15)")
                        .HasComment("母單憑證狀態");

                    b.Property<string>("combo_model")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("jsonb")
                        .HasDefaultValueSql("'{}'::jsonb")
                        .HasComment("商品模組");

                    b.Property<DateTime>("create_datetime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("now()")
                        .HasComment("建立時間");

                    b.Property<string>("create_user")
                        .IsRequired()
                        .HasColumnType("varchar(36)")
                        .HasComment("建立者");

                    b.Property<string>("go_date")
                        .HasColumnType("varchar(8)")
                        .HasComment("出發日期");

                    b.Property<bool?>("is_back")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasComment("是否已調整母單為back")
                        .HasDefaultValue(false);

                    b.Property<bool?>("is_callback")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasComment("是否已callback")
                        .HasDefaultValue(false);

                    b.Property<bool?>("is_need_back")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasComment("是否需要調整母單")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("modify_datetime")
                        .HasColumnType("timestamp without time zone")
                        .HasComment("修改時間");

                    b.Property<string>("modify_user")
                        .HasColumnType("varchar(36)")
                        .HasComment("修改者");

                    b.Property<DateTime?>("monitor_start_datetime")
                        .HasColumnType("timestamp without time zone")
                        .HasComment("憑證取得起始時間");

                    b.Property<string>("order_mid")
                        .HasColumnType("varchar(13)")
                        .HasComment("order_mid");

                    b.Property<int?>("order_oid")
                        .HasColumnType("int4")
                        .HasComment("order_oid");

                    b.Property<int?>("prod_oid")
                        .HasColumnType("int4")
                        .HasComment("prod_oid");

                    b.Property<int?>("voucher_deadline")
                        .HasColumnType("int4")
                        .HasComment("取憑證效期");

                    b.HasKey("booking_mst_xid");

                    b.ToTable("booking_mst");
                });

            modelBuilder.Entity("OCBT.Infra.DAL.Migrations.OCBTContext+combo_supplier", b =>
                {
                    b.Property<long>("combo_supplier_xid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int8")
                        .HasComment("序號")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime>("create_datetime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("now()")
                        .HasComment("建立時間");

                    b.Property<string>("create_user")
                        .IsRequired()
                        .HasColumnType("varchar(36)")
                        .HasComment("建立者");

                    b.Property<DateTime?>("modify_datetime")
                        .HasColumnType("timestamp without time zone")
                        .HasComment("修改時間");

                    b.Property<string>("modify_user")
                        .HasColumnType("varchar(36)")
                        .HasComment("修改者");

                    b.Property<string>("status")
                        .HasColumnType("varchar(12)")
                        .HasComment("狀態");

                    b.Property<int?>("supplier_oid")
                        .HasColumnType("int4")
                        .HasComment("供應商序號");

                    b.HasKey("combo_supplier_xid");

                    b.ToTable("combo_supplier");
                });
#pragma warning restore 612, 618
        }
    }
}