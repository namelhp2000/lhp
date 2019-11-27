using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoadFlow.Model;
using System;
using System.Text;
using Xunit;

namespace UnitTestUtilty
{
    [TestClass]
    public class MapperTest
    {
        [TestMethod]
        [Fact]
        public async void GetDataTableTest()
        {


            string ConnType = "sqlserver";
            string ConnString = "server=localhost;database=RoadFlowCore;uid=sa;pwd=sa"; //"server=192.168.10.198;database=SBO;uid=sa;pwd=123456Zz";
            RoadFlow.Mapper.DataContext db = new RoadFlow.Mapper.DataContext(ConnType, ConnString);
            string sql = $@"--declare @p_stime varchar(20) -- ��ʼ
--declare @p_etime varchar(20) -- ����
--declare @p_istsbz int -- �Ƿ����ⲡ��

--set @p_stime = '2015-05-01'
--set @p_etime = '2015-05-11'
--set @p_istsbz = 0
---n_serial_case�����ݿ��Ҳ����������ֶ�
SET ANSI_WARNINGS OFF
SELECT
	0 AS mark,
	ISNULL(pcase.s_doctor_input, pomd.s_doctor_input) AS s_doctor_input, -- ҽ��
	po.DEPT_NAME , -- ����
	SUM(CASE WHEN (pomd.s_sort_code='01' AND pomd.tp=1) THEN pomd.n_money ELSE 0 END) AS sum_xiy, -- ��ҩ
	SUM(CASE WHEN (pomd.s_sort_code='02' AND pomd.tp=1) THEN pomd.n_money ELSE 0 END) AS sum_cny, -- ��ҩ
	SUM(CASE WHEN (pomd.s_sort_code='03' AND pomd.tp=1) THEN pomd.n_money ELSE 0 END) AS sum_coy, -- ��ҩ
	SUM(CASE WHEN pomd.n_jb=1 THEN pomd.n_money ELSE 0 END) AS sum_jby, -- ����ҩ
	SUM(CASE WHEN pomd.n_jb=1 AND pomd.s_man_kind='1' THEN pomd.n_money ELSE 0 END) AS sum_yb_jby, -- ҽ������ҩ
	SUM(CASE WHEN pomd.n_jb=1 AND pomd.s_man_kind='3' THEN pomd.n_money ELSE 0 END) AS sum_nb_jby, -- ũ������ҩ
	SUM(CASE WHEN pomd.s_item = '������' THEN pomd.n_money ELSE 0 END) AS sum_klj, -- ����������
	SUM(CASE WHEN pomd.s_item = 'С��װ' THEN pomd.n_money ELSE 0 END) AS sum_xbz, -- С��װ����
	COUNT(DISTINCT CASE WHEN pomd.n_grade>0 THEN pomd.s_ic_no ELSE null END) AS cnt_kss, -- �������˴�
	SUM(CASE WHEN pomd.n_grade>0 THEN pomd.n_money ELSE 0 END) AS sum_kss, -- �����ط���
	SUM(CASE WHEN pomd.s_yb_mark<>1 THEN pomd.n_money ELSE 0 END) AS sum_noyb, -- ��ҽ��
	SUM(CASE WHEN pomd.tp=1 THEN pomd.n_money ELSE 0 END) AS sum_amd, -- ҩƷ�ܷ���
	SUM(pomd.n_money) AS sum_all -- ȫ������
FROM

	(
		-- ����ҩƷ
		SELECT 1 AS tp, md.s_ic_no,
			'' n_serial_case, md.d_time_charge AS d_time, s_doctor_input,s_office_input as s_office_doctor,
			md.n_money,
			md.s_sort_code, md.s_sort_item AS s_item,
			yb.n_kind AS n_jb, pa.s_man_kind, ybinfo.s_yb_mark, ycg.n_grade
		FROM
			HIS_Cost_Bill_Medicine_Out AS md
			INNER JOIN
			HIS_Account pa ON pa.s_ic_no=md.s_ic_no
			LEFT OUTER JOIN
			HIS_Basic_Medicine AS yb ON yb.s_cost_no = md.s_cost_no AND yb.s_pharmacy=md.s_office_excute
			LEFT OUTER JOIN
			(
				SELECT ybc.s_cost_no as s_fy_no, ybc.s_cost_kind as s_yb_kind, ybb.n_mark as s_yb_mark, ybb.s_cost_no as s_fy_no_comp
				FROM
					HIS_Basic_Cost_Medicare_Connect AS ybc
					INNER JOIN
					HIS_Basic_Cost_Medicare AS ybb ON
						ybb.s_cost_no = ybc.s_cost_no AND ybb.s_kind = ybc.s_cost_kind
			) AS ybinfo ON 
				ybinfo.s_fy_no = md.s_cost_no AND
				(
					pa.s_man_kind='1' AND ybinfo.s_yb_kind ='C' OR
					pa.s_man_kind='3' AND ybinfo.s_yb_kind ='N'
				)
			LEFT OUTER JOIN
			HIS_Basic_Medicine_Info AS ycg ON ycg.s_cost_no=md.s_cost_no
			--0��ʾ�ǿ�����1��ʾ�� 2��ʾ�� 3��ʾ����
		WHERE n_mark_charge='1'
	UNION ALL
		-- ������
		SELECT 2 as tp, s_ic_no,
			'' n_serial_case, d_time_charge AS d_time, s_doctor_input,s_office_input as s_office_doctor,
			n_money,
			s_sort_code, s_office_excute AS s_item,
			null AS n_jb, null AS s_man_kind, null AS s_yb_mark, null AS n_grade
		FROM HIS_Cost_Bill_Check_Out
		WHERE n_mark_charge='1'
	UNION ALL
		-- ��������
		SELECT 3 as tp, s_ic_no,
		 ''	n_serial_case, d_time_input AS d_time, s_doctor_input,s_office_input as s_office_doctor,
			n_money,
			s_sort_code, 'zl' AS s_item,
			null AS n_jb, null AS s_man_kind, null AS s_yb_mark, null AS n_grade
		FROM HIS_Cost_Bill_Cure_Out
		WHERE n_mark_charge='1'
	) AS pomd




	LEFT OUTER JOIN
	( -- ҽ������õĹ�ϵ
		SELECT poca.s_ic_no, poca.s_doctor_input
		FROM	HIS_Case_Out AS poca
	) AS pcase ON pcase.s_ic_no=pomd.s_ic_no



	INNER JOIN

	( -- ��ȡҽ���ĵ�һ��Ϣ
		SELECT * FROM (SELECT
				ROW_NUMBER() OVER (PARTITION BY NAME ORDER BY n_active DESC,d_input_time) AS rowidx, *
			FROM SYS_USER ) AS puser WHERE rowidx=1
	) AS pu ON pu.NAME=ISNULL(pcase.s_doctor_input, pomd.s_doctor_input)


	INNER JOIN
	SYS_DEPARTMENT AS po on pu.s_office=po.DEPT_CODE

	LEFT OUTER JOIN
	( -- ���ⲡ��
		SELECT DISTINCT s_ic_no
		FROM HIS_Account_Medicare_Disease
		WHERE (d_time_input BETWEEN '2018/8/1 0:00:00' AND '2019/9/1 0:00:00')
			AND s_code NOT LIKE '999%'
	) AS pads ON pcase.s_ic_no = pads.s_ic_no
	 

WHERE pomd.d_time BETWEEN '2018/8/1 0:00:00' AND '2019/9/1 0:00:00'
	AND (
		(0=1 AND pads.s_ic_no IS NOT NULL) OR
		(0=0 AND pads.s_ic_no IS NULL) OR
		(0=2)
	)
GROUP BY ISNULL(pcase.s_doctor_input, pomd.s_doctor_input), po.DEPT_NAME
HAVING SUM(CASE WHEN pomd.tp=1 THEN pomd.n_money ELSE 0 END)>0
ORDER BY po.DEPT_NAME;";

            string sql1 = "select *  from RF_Program";
            //  var table= await   db.QueryAllAsync<RoadFlow.Model.Program>();
            //  var table1 = await db.GetDataTableAsync(sql);
            var table1 = await db.QueryAsync<RoadFlow.Model.Menu>("SELECT * FROM RF_Menu WHERE 1=1");
            var ss = "";
        }



        [Fact]
        public async void GetTableTestAsync()
        {
            string ConnType = "sqlserver";
            string ConnString = "server=192.168.10.198;database=SBO;uid=sa;pwd=123456Zz";
            RoadFlow.Mapper.DataContext db = new RoadFlow.Mapper.DataContext(ConnType, ConnString);
            string sql = $@"--declare @p_stime varchar(20) -- ��ʼ
--declare @p_etime varchar(20) -- ����
--declare @p_istsbz int -- �Ƿ����ⲡ��

--set @p_stime = '2015-05-01'
--set @p_etime = '2015-05-11'
--set @p_istsbz = 0
---n_serial_case�����ݿ��Ҳ����������ֶ�
SET ANSI_WARNINGS OFF
SELECT
	0 AS mark,
	ISNULL(pcase.s_doctor_input, pomd.s_doctor_input) AS s_doctor_input, -- ҽ��
	po.DEPT_NAME , -- ����
	SUM(CASE WHEN (pomd.s_sort_code='01' AND pomd.tp=1) THEN pomd.n_money ELSE 0 END) AS sum_xiy, -- ��ҩ
	SUM(CASE WHEN (pomd.s_sort_code='02' AND pomd.tp=1) THEN pomd.n_money ELSE 0 END) AS sum_cny, -- ��ҩ
	SUM(CASE WHEN (pomd.s_sort_code='03' AND pomd.tp=1) THEN pomd.n_money ELSE 0 END) AS sum_coy, -- ��ҩ
	SUM(CASE WHEN pomd.n_jb=1 THEN pomd.n_money ELSE 0 END) AS sum_jby, -- ����ҩ
	SUM(CASE WHEN pomd.n_jb=1 AND pomd.s_man_kind='1' THEN pomd.n_money ELSE 0 END) AS sum_yb_jby, -- ҽ������ҩ
	SUM(CASE WHEN pomd.n_jb=1 AND pomd.s_man_kind='3' THEN pomd.n_money ELSE 0 END) AS sum_nb_jby, -- ũ������ҩ
	SUM(CASE WHEN pomd.s_item = '������' THEN pomd.n_money ELSE 0 END) AS sum_klj, -- ����������
	SUM(CASE WHEN pomd.s_item = 'С��װ' THEN pomd.n_money ELSE 0 END) AS sum_xbz, -- С��װ����
	COUNT(DISTINCT CASE WHEN pomd.n_grade>0 THEN pomd.s_ic_no ELSE null END) AS cnt_kss, -- �������˴�
	SUM(CASE WHEN pomd.n_grade>0 THEN pomd.n_money ELSE 0 END) AS sum_kss, -- �����ط���
	SUM(CASE WHEN pomd.s_yb_mark<>1 THEN pomd.n_money ELSE 0 END) AS sum_noyb, -- ��ҽ��
	SUM(CASE WHEN pomd.tp=1 THEN pomd.n_money ELSE 0 END) AS sum_amd, -- ҩƷ�ܷ���
	SUM(pomd.n_money) AS sum_all -- ȫ������
FROM

	(
		-- ����ҩƷ
		SELECT 1 AS tp, md.s_ic_no,
			'' n_serial_case, md.d_time_charge AS d_time, s_doctor_input,s_office_input as s_office_doctor,
			md.n_money,
			md.s_sort_code, md.s_sort_item AS s_item,
			yb.n_kind AS n_jb, pa.s_man_kind, ybinfo.s_yb_mark, ycg.n_grade
		FROM
			HIS_Cost_Bill_Medicine_Out AS md
			INNER JOIN
			HIS_Account pa ON pa.s_ic_no=md.s_ic_no
			LEFT OUTER JOIN
			HIS_Basic_Medicine AS yb ON yb.s_cost_no = md.s_cost_no AND yb.s_pharmacy=md.s_office_excute
			LEFT OUTER JOIN
			(
				SELECT ybc.s_cost_no as s_fy_no, ybc.s_cost_kind as s_yb_kind, ybb.n_mark as s_yb_mark, ybb.s_cost_no as s_fy_no_comp
				FROM
					HIS_Basic_Cost_Medicare_Connect AS ybc
					INNER JOIN
					HIS_Basic_Cost_Medicare AS ybb ON
						ybb.s_cost_no = ybc.s_cost_no AND ybb.s_kind = ybc.s_cost_kind
			) AS ybinfo ON 
				ybinfo.s_fy_no = md.s_cost_no AND
				(
					pa.s_man_kind='1' AND ybinfo.s_yb_kind ='C' OR
					pa.s_man_kind='3' AND ybinfo.s_yb_kind ='N'
				)
			LEFT OUTER JOIN
			HIS_Basic_Medicine_Info AS ycg ON ycg.s_cost_no=md.s_cost_no
			--0��ʾ�ǿ�����1��ʾ�� 2��ʾ�� 3��ʾ����
		WHERE n_mark_charge='1'
	UNION ALL
		-- ������
		SELECT 2 as tp, s_ic_no,
			'' n_serial_case, d_time_charge AS d_time, s_doctor_input,s_office_input as s_office_doctor,
			n_money,
			s_sort_code, s_office_excute AS s_item,
			null AS n_jb, null AS s_man_kind, null AS s_yb_mark, null AS n_grade
		FROM HIS_Cost_Bill_Check_Out
		WHERE n_mark_charge='1'
	UNION ALL
		-- ��������
		SELECT 3 as tp, s_ic_no,
		 ''	n_serial_case, d_time_input AS d_time, s_doctor_input,s_office_input as s_office_doctor,
			n_money,
			s_sort_code, 'zl' AS s_item,
			null AS n_jb, null AS s_man_kind, null AS s_yb_mark, null AS n_grade
		FROM HIS_Cost_Bill_Cure_Out
		WHERE n_mark_charge='1'
	) AS pomd




	LEFT OUTER JOIN
	( -- ҽ������õĹ�ϵ
		SELECT poca.s_ic_no, poca.s_doctor_input
		FROM	HIS_Case_Out AS poca
	) AS pcase ON pcase.s_ic_no=pomd.s_ic_no



	INNER JOIN

	( -- ��ȡҽ���ĵ�һ��Ϣ
		SELECT * FROM (SELECT
				ROW_NUMBER() OVER (PARTITION BY NAME ORDER BY n_active DESC,d_input_time) AS rowidx, *
			FROM SYS_USER ) AS puser WHERE rowidx=1
	) AS pu ON pu.NAME=ISNULL(pcase.s_doctor_input, pomd.s_doctor_input)


	INNER JOIN
	SYS_DEPARTMENT AS po on pu.s_office=po.DEPT_CODE

	LEFT OUTER JOIN
	( -- ���ⲡ��
		SELECT DISTINCT s_ic_no
		FROM HIS_Account_Medicare_Disease
		WHERE (d_time_input BETWEEN '2018/8/1 0:00:00' AND '2019/9/1 0:00:00')
			AND s_code NOT LIKE '999%'
	) AS pads ON pcase.s_ic_no = pads.s_ic_no
	 

WHERE pomd.d_time BETWEEN '2018/8/1 0:00:00' AND '2019/9/1 0:00:00'
	AND (
		(0=1 AND pads.s_ic_no IS NOT NULL) OR
		(0=0 AND pads.s_ic_no IS NULL) OR
		(0=2)
	)
GROUP BY ISNULL(pcase.s_doctor_input, pomd.s_doctor_input), po.DEPT_NAME
HAVING SUM(CASE WHEN pomd.tp=1 THEN pomd.n_money ELSE 0 END)>0
ORDER BY po.DEPT_NAME;";

          
           
            var table1 = await db.GetDataTableAsync(sql);
        
            var ss = "";



    
        }


    }
}
