using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KKday.Web.OCBT.Migrations
{
    public partial class InitialCreate_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "booking_dtl",
                columns: table => new
                {
                    booking_dtl_xid = table.Column<long>(type: "int8", nullable: false, comment: "序號")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    create_user = table.Column<string>(type: "varchar(36)", nullable: false, comment: "建立者"),
                    create_datetime = table.Column<DateTime>(nullable: false, defaultValueSql: "now()", comment: "建立時間"),
                    modify_user = table.Column<string>(type: "varchar(36)", nullable: true, comment: "修改者"),
                    modify_datetime = table.Column<DateTime>(nullable: true, comment: "修改時間"),
                    booking_mst_xid = table.Column<long>(type: "int8", nullable: true, comment: "booking_mst_xid"),
                    prod_oid = table.Column<int>(type: "int4", nullable: true, comment: "prod_oid"),
                    package_oid = table.Column<int>(type: "int4", nullable: true, comment: "package_oid"),
                    item_oid = table.Column<int>(type: "int4", nullable: true, comment: "item_oid"),
                    sku_oid = table.Column<string>(type: "jsonb", nullable: true, defaultValueSql: "'{}'::jsonb", comment: "sku_oid"),
                    real_booking_qty = table.Column<int>(type: "int4", nullable: true, comment: "實際訂購數"),
                    voucher_file_info = table.Column<string>(type: "jsonb", nullable: true, defaultValueSql: "'[]'::jsonb", comment: "voucher檔案"),
                    order_master_oid = table.Column<int>(type: "int4", nullable: true, comment: "order_master_oid"),
                    order_master_mid = table.Column<string>(type: "varchar(13)", nullable: true, comment: "order_master_mid"),
                    order_oid = table.Column<int>(type: "int4", nullable: true, comment: "order_oid"),
                    order_mid = table.Column<string>(type: "varchar(13)", nullable: true, comment: "order_mid"),
                    booking_dtl_order_status = table.Column<string>(type: "varchar(15)", nullable: true, comment: "子單訂單狀態"),
                    booking_dtl_voucher_status = table.Column<string>(type: "varchar(15)", nullable: true, comment: "子單憑證狀態")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_dtl", x => x.booking_dtl_xid);
                });

            migrationBuilder.CreateTable(
                name: "booking_log",
                columns: table => new
                {
                    booking_log_xid = table.Column<long>(type: "int8", nullable: false, comment: "序號")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    create_user = table.Column<string>(type: "varchar(36)", nullable: false, comment: "建立者"),
                    create_datetime = table.Column<DateTime>(nullable: false, defaultValueSql: "now()", comment: "建立時間"),
                    modify_user = table.Column<string>(type: "varchar(36)", nullable: true, comment: "修改者"),
                    modify_datetime = table.Column<DateTime>(nullable: true, comment: "修改時間"),
                    log_type = table.Column<string>(type: "varchar(5)", nullable: true, comment: "log類型"),
                    booking_mst_xid = table.Column<long>(type: "int8", nullable: true, comment: "booking_mst_xid"),
                    booking_dtl_xid = table.Column<long>(type: "int8", nullable: true, comment: "booking_dtl_xid"),
                    booking_source = table.Column<string>(type: "varchar(5)", nullable: true, comment: "來源為MST/DTL"),
                    status_type = table.Column<string>(type: "varchar(15)", nullable: true, comment: "類型為ORDER/VOUCHER"),
                    status = table.Column<string>(type: "varchar(20)", nullable: true, comment: "狀態"),
                    previous_status = table.Column<string>(type: "jsonb", nullable: true, comment: "上一個狀態")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_log", x => x.booking_log_xid);
                });

            migrationBuilder.CreateTable(
                name: "booking_mst",
                columns: table => new
                {
                    booking_mst_xid = table.Column<long>(type: "int8", nullable: false, comment: "序號")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    create_user = table.Column<string>(type: "varchar(36)", nullable: false, comment: "建立者"),
                    create_datetime = table.Column<DateTime>(nullable: false, defaultValueSql: "now()", comment: "建立時間"),
                    modify_user = table.Column<string>(type: "varchar(36)", nullable: true, comment: "修改者"),
                    modify_datetime = table.Column<DateTime>(nullable: true, comment: "修改時間"),
                    order_mid = table.Column<string>(type: "varchar(13)", nullable: true, comment: "order_mid"),
                    order_oid = table.Column<int>(type: "int4", nullable: true, comment: "order_oid"),
                    prod_oid = table.Column<int>(type: "int4", nullable: true, comment: "prod_oid"),
                    go_date = table.Column<string>(type: "varchar(8)", nullable: true, comment: "出發日期"),
                    booking_model = table.Column<string>(type: "jsonb", nullable: true, defaultValueSql: "'{}'::jsonb", comment: "訂購模組"),
                    combo_model = table.Column<string>(type: "jsonb", nullable: true, defaultValueSql: "'{}'::jsonb", comment: "商品模組"),
                    booking_mst_order_status = table.Column<string>(type: "varchar(15)", nullable: true, comment: "母單訂單狀態"),
                    booking_mst_voucher_status = table.Column<string>(type: "varchar(15)", nullable: true, comment: "母單憑證狀態"),
                    voucher_deadline = table.Column<int>(type: "int4", nullable: true, comment: "取憑證效期"),
                    is_callback = table.Column<bool>(type: "bool", nullable: true, defaultValue: false, comment: "是否已callback"),
                    is_back = table.Column<bool>(type: "bool", nullable: true, defaultValue: false, comment: "是否已調整母單為back"),
                    is_need_back = table.Column<bool>(type: "bool", nullable: true, defaultValue: true, comment: "是否需要調整母單"),
                    monitor_start_datetime = table.Column<DateTime>(nullable: true, comment: "憑證取得起始時間")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_mst", x => x.booking_mst_xid);
                });

            migrationBuilder.CreateTable(
                name: "combo_supplier",
                columns: table => new
                {
                    combo_supplier_xid = table.Column<long>(type: "int8", nullable: false, comment: "序號")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    create_user = table.Column<string>(type: "varchar(36)", nullable: false, comment: "建立者"),
                    create_datetime = table.Column<DateTime>(nullable: false, defaultValueSql: "now()", comment: "建立時間"),
                    modify_user = table.Column<string>(type: "varchar(36)", nullable: true, comment: "修改者"),
                    modify_datetime = table.Column<DateTime>(nullable: true, comment: "修改時間"),
                    supplier_oid = table.Column<int>(type: "int4", nullable: true, comment: "供應商序號"),
                    status = table.Column<string>(type: "varchar(12)", nullable: true, comment: "狀態")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_combo_supplier", x => x.combo_supplier_xid);
                });

            #region booking_dtl 連動 log



            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION public.booking_dtl_upd_trigger()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
    declare
      param1Temp  jsonb; 
    begin
 	    
 	    if (TG_OP = 'UPDATE' )
 	    then
 	      if( old.booking_dtl_order_status <> new.booking_dtl_order_status)
 	      then
 	        select 
 	        JSONB_BUILD_OBJECT('old_booking_dtl_order_status',old.booking_dtl_order_status
            )::jsonb 
 	        into param1Temp ;
 	        INSERT INTO public.booking_log
            (log_type,booking_mst_xid,booking_dtl_xid,booking_source,status_type,status, previous_status,  create_user, create_datetime)
            VALUES('03',old.booking_mst_xid,old.booking_dtl_xid,'DTL','ORDER',new.booking_dtl_order_status, param1Temp, 'SYSTEM', now());
 	      end if;
 	      if( old.booking_dtl_voucher_status <> new.booking_dtl_voucher_status)
 	      then
 	        select 
 	        JSONB_BUILD_OBJECT('old_booking_dtl_voucher_status',old.booking_dtl_order_status
            )::jsonb 
 	        into param1Temp ;
 	        INSERT INTO public.booking_log
            (log_type,booking_mst_xid,booking_dtl_xid,booking_source,status_type,status, previous_status,  create_user, create_datetime)
            VALUES('04',old.booking_mst_xid,old.booking_dtl_xid,'DTL','VOUCHER',new.booking_dtl_voucher_status, param1Temp, 'SYSTEM', now());
 	      end if;
 	     end if;
        RETURN new;
    END;
$function$");

            migrationBuilder.Sql(@"create trigger booking_dtl_trigger before update on booking_dtl for each row execute PROCEDURE booking_dtl_upd_trigger()");


            #endregion

            #region booking_mst 連動 log



            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION public.booking_mst_upd_trigger()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
    declare
      param1Temp  jsonb; 
    begin
 	    
 	    if (TG_OP = 'UPDATE' )
 	    then
 	      if( old.booking_mst_order_status <> new.booking_mst_order_status)
 	      then
 	        select 
 	        JSONB_BUILD_OBJECT('old_booking_mst_order_status',old.booking_mst_order_status
            )::jsonb 
 	        into param1Temp ;
 	        INSERT INTO public.booking_log
            (log_type,booking_mst_xid,booking_source,status_type,status, previous_status,  create_user, create_datetime)
            VALUES('01',old.booking_mst_xid,'MST','ORDER',new.booking_mst_order_status, param1Temp, 'SYSTEM', now());
 	      end if;
 	      if( old.booking_mst_voucher_status <> new.booking_mst_voucher_status)
 	      then
 	        select 
 	        JSONB_BUILD_OBJECT('old_booking_mst_voucher_status',old.booking_mst_order_status
            )::jsonb 
 	        into param1Temp ;
 	        INSERT INTO public.booking_log
            (log_type,booking_mst_xid,booking_source,status_type,status, previous_status,  create_user, create_datetime)
            VALUES('02',old.booking_mst_xid,'MST','VOUCHER',new.booking_mst_voucher_status, param1Temp, 'SYSTEM', now());
 	      end if;
 	     end if;
 	     if( old.is_back <> new.is_back)
 	     then 
 	      select 
 	        JSONB_BUILD_OBJECT('old_booking_mst_is_back',old.is_back
            )::jsonb 
            into param1Temp ;
 	      INSERT INTO public.booking_log
            (log_type,booking_mst_xid,booking_source,status_type,status, previous_status,  create_user, create_datetime)
            VALUES('05',old.booking_mst_xid,'MST','IS_BACK',new.is_back::text, param1Temp, 'SYSTEM', now());
 	     end if;
 	    if( old.is_need_back <> new.is_need_back)
 	     then 
 	      select 
 	        JSONB_BUILD_OBJECT('old_is_need_back',old.is_need_back
            )::jsonb 
            into param1Temp ;
 	      INSERT INTO public.booking_log
            (log_type,booking_mst_xid,booking_source,status_type,status, previous_status,  create_user, create_datetime)
            VALUES('06',old.booking_mst_xid,'MST','IS_NEED_BACK',new.is_need_back::text, param1Temp, 'SYSTEM', now());
 	     end if;
 	    if( old.is_callback <> new.is_callback)
 	     then 
 	      select 
 	        JSONB_BUILD_OBJECT('old_is_callback',old.is_callback
            )::jsonb 
            into param1Temp ;
 	      INSERT INTO public.booking_log
            (log_type,booking_mst_xid,booking_source,status_type,status, previous_status,  create_user, create_datetime)
            VALUES('07',old.booking_mst_xid,'MST','IS_CALLBACK',new.is_callback::text, param1Temp, 'SYSTEM', now());
 	     end if;
 	    
        RETURN new;
    END;
$function$");

            migrationBuilder.Sql(@"create trigger booking_mst_trigger before update on public.booking_mst for each row execute PROCEDURE booking_mst_upd_trigger()");

            #endregion 
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "booking_dtl");

            migrationBuilder.DropTable(
                name: "booking_log");

            migrationBuilder.DropTable(
                name: "booking_mst");

            migrationBuilder.DropTable(
                name: "combo_supplier");
        }
    }
}
