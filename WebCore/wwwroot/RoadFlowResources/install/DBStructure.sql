
/****** Object:  Table [dbo].[TaskLog]    Script Date: 11/02/2019 14:39:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskLog](
	[Id] [uniqueidentifier] NOT NULL,
	[TaskId] [uniqueidentifier] NULL,
	[TaskName] [nvarchar](50) NULL,
	[BeginDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Msg] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaskLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'作业名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskLog', @level2type=N'COLUMN',@level2name=N'TaskName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'作业开始时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskLog', @level2type=N'COLUMN',@level2name=N'BeginDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'作业结束时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskLog', @level2type=N'COLUMN',@level2name=N'EndDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'作业信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskLog', @level2type=N'COLUMN',@level2name=N'Msg'
GO
/****** Object:  StoredProcedure [dbo].[sp_tableDict]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_tableDict]
(
    @tableName AS VARCHAR(100)
)
    --@parameter_name AS scalar_data_type ( = default_value ), ...
-- WITH ENCRYPTION, RECOMPILE, EXECUTE AS CALLER|SELF|OWNER| 'user_name'
AS
SELECT  表名 = CASE WHEN a.colorder = 1 THEN d.name
                  ELSE ''
             END ,
        表说明 = CASE WHEN a.colorder = 1 THEN ISNULL(f.value, '')
                   ELSE ''
              END ,
        字段序号 = a.colorder ,
        字段名 = a.name ,
        标识 = CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1 THEN '√'
                  ELSE ''
             END ,
        主键 = CASE WHEN EXISTS ( SELECT  1
                                FROM    sysobjects
                                WHERE   xtype = 'PK'
                                        AND name IN ( SELECT  name
                                                      FROM    sysindexes
                                                      WHERE   indid IN ( SELECT indid
                                                                         FROM   sysindexkeys
                                                                         WHERE  id = a.id
                                                                                AND colid = a.colid ) ) ) THEN '√'
                  ELSE ''
             END ,
        类型 = b.name ,
        占用字节数 = a.length ,
        长度 = COLUMNPROPERTY(a.id, a.name, 'PRECISION') ,
        小数位数 = ISNULL(COLUMNPROPERTY(a.id, a.name, 'Scale'), 0) ,
        允许空 = CASE WHEN a.isnullable = 1 THEN '√'
                   ELSE ''
              END ,
        默认值 = ISNULL(e.text, '') ,
        字段说明 = ISNULL(g.[value], '')
FROM    syscolumns a
        LEFT  JOIN systypes b
        ON a.xtype = b.xusertype
        INNER  JOIN sysobjects d
        ON a.id = d.id
           AND d.xtype = 'U'
           AND d.name <> 'dtproperties'
        LEFT  JOIN syscomments e
        ON a.cdefault = e.id
        LEFT  JOIN sys.extended_properties g
        ON a.id = g.major_id
           AND a.colid = g.minor_id
        LEFT  JOIN sys.extended_properties f
        ON d.id = f.major_id
           AND f.minor_id = 0
WHERE   d.name = @tableName
ORDER BY a.id ,
        a.colorder
GO
/****** Object:  Table [dbo].[RF_WorkGroup]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_WorkGroup](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Members] [varchar](max) NULL,
	[Note] [nvarchar](200) NULL,
	[Sort] [int] NOT NULL,
	[IntId] [int] NOT NULL,
 CONSTRAINT [PK_RF_WorkGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'工作组Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_WorkGroup', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'工作组名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_WorkGroup', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'工作组成员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_WorkGroup', @level2type=N'COLUMN',@level2name=N'Members'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_WorkGroup', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_WorkGroup', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数字ID，用于微信等其它第三方' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_WorkGroup', @level2type=N'COLUMN',@level2name=N'IntId'
GO
/****** Object:  Table [dbo].[RF_WorkDate]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RF_WorkDate](
	[WorkDay] [date] NOT NULL,
	[IsWork] [int] NOT NULL,
 CONSTRAINT [PK_RF_WorkDate] PRIMARY KEY CLUSTERED 
(
	[WorkDay] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'工作日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_WorkDate', @level2type=N'COLUMN',@level2name=N'WorkDay'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0节假日 1工作日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_WorkDate', @level2type=N'COLUMN',@level2name=N'IsWork'
GO
/****** Object:  Table [dbo].[RF_VoteResultUser]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RF_VoteResultUser](
	[Id] [uniqueidentifier] NOT NULL,
	[VoteId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_RF_VoteResultUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'说明', @value=N'可以查看结果的用户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteResultUser'
GO
/****** Object:  Table [dbo].[RF_VoteResult]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_VoteResult](
	[Id] [uniqueidentifier] NOT NULL,
	[VoteId] [uniqueidentifier] NOT NULL,
	[ItemId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[OptionId] [uniqueidentifier] NOT NULL,
	[OptionOther] [varchar](2000) NULL,
 CONSTRAINT [PK_RF_VoteResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'投票ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteResult', @level2type=N'COLUMN',@level2name=N'VoteId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'选题ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteResult', @level2type=N'COLUMN',@level2name=N'ItemId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteResult', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'选项ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteResult', @level2type=N'COLUMN',@level2name=N'OptionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'其它说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteResult', @level2type=N'COLUMN',@level2name=N'OptionOther'
GO
/****** Object:  Table [dbo].[RF_VotePartakeUser]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RF_VotePartakeUser](
	[Id] [uniqueidentifier] NOT NULL,
	[VoteId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[UserOrganize] [nvarchar](2000) NOT NULL,
	[Status] [int] NOT NULL,
	[SubmitTime] [datetime] NULL,
 CONSTRAINT [PK_RF_VotePartakeUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VotePartakeUser', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户所在组织' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VotePartakeUser', @level2type=N'COLUMN',@level2name=N'UserOrganize'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态 0未提交 1已提交' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VotePartakeUser', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'提交时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VotePartakeUser', @level2type=N'COLUMN',@level2name=N'SubmitTime'
GO
EXEC sys.sp_addextendedproperty @name=N'说明', @value=N'参与调查的用户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VotePartakeUser'
GO
/****** Object:  Table [dbo].[RF_VoteItemOption]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_VoteItemOption](
	[Id] [uniqueidentifier] NOT NULL,
	[VoteId] [uniqueidentifier] NOT NULL,
	[ItemId] [uniqueidentifier] NOT NULL,
	[OptionTitle] [varchar](500) NOT NULL,
	[IsInput] [int] NOT NULL,
	[InputStyle] [varchar](500) NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_VoteItemOption] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'选项标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteItemOption', @level2type=N'COLUMN',@level2name=N'OptionTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'输入 1文本框 2文本域 0无' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteItemOption', @level2type=N'COLUMN',@level2name=N'IsInput'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'输入框样式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteItemOption', @level2type=N'COLUMN',@level2name=N'InputStyle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteItemOption', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_VoteItem]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_VoteItem](
	[Id] [uniqueidentifier] NOT NULL,
	[VoteId] [uniqueidentifier] NOT NULL,
	[ItemTitle] [varchar](1000) NOT NULL,
	[SelectModel] [int] NOT NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_VoteItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteItem', @level2type=N'COLUMN',@level2name=N'ItemTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'选择方式 0 不是选择 1 单选 2多选' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteItem', @level2type=N'COLUMN',@level2name=N'SelectModel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_VoteItem', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_Vote]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Vote](
	[Id] [uniqueidentifier] NOT NULL,
	[Topic] [varchar](500) NOT NULL,
	[CreateUserId] [uniqueidentifier] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[PartakeUsers] [varchar](max) NOT NULL,
	[ResultViewUsers] [varchar](max) NULL,
	[EndTime] [datetime] NOT NULL,
	[Note] [varchar](500) NULL,
	[Status] [int] NOT NULL,
	[PublishTime] [datetime] NULL,
	[Anonymous] [int] NOT NULL,
 CONSTRAINT [PK_RF_Vote] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Vote', @level2type=N'COLUMN',@level2name=N'Topic'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发起人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Vote', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Vote', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参与人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Vote', @level2type=N'COLUMN',@level2name=N'PartakeUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'结果查看人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Vote', @level2type=N'COLUMN',@level2name=N'ResultViewUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'结束时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Vote', @level2type=N'COLUMN',@level2name=N'EndTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Vote', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态 0设计中 1已发布 2已有结果 3已完成(所有人提交)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Vote', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否匿名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Vote', @level2type=N'COLUMN',@level2name=N'Anonymous'
GO
/****** Object:  Table [dbo].[RF_UserShortcut]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RF_UserShortcut](
	[Id] [uniqueidentifier] NOT NULL,
	[MenuId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_UserShortcut] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户快捷方式Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserShortcut', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'菜单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserShortcut', @level2type=N'COLUMN',@level2name=N'MenuId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserShortcut', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserShortcut', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_UserFileShare]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_UserFileShare](
	[FileId] [varchar](800) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[FileName] [varchar](500) NOT NULL,
	[ShareUserId] [uniqueidentifier] NOT NULL,
	[ShareDate] [datetime] NOT NULL,
	[ExpireDate] [datetime] NOT NULL,
	[IsView] [int] NOT NULL,
 CONSTRAINT [PK_RF_UserFileShare] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserFileShare', @level2type=N'COLUMN',@level2name=N'FileId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'人员ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserFileShare', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserFileShare', @level2type=N'COLUMN',@level2name=N'FileName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分享的人员ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserFileShare', @level2type=N'COLUMN',@level2name=N'ShareUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分享日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserFileShare', @level2type=N'COLUMN',@level2name=N'ShareDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'过期时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserFileShare', @level2type=N'COLUMN',@level2name=N'ExpireDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否查看' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_UserFileShare', @level2type=N'COLUMN',@level2name=N'IsView'
GO
/****** Object:  Table [dbo].[RF_User]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_User](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Account] [varchar](255) NOT NULL,
	[Password] [varchar](200) NOT NULL,
	[Sex] [int] NULL,
	[Status] [int] NOT NULL,
	[Job] [varchar](200) NULL,
	[Note] [nvarchar](500) NULL,
	[Mobile] [varchar](50) NULL,
	[Tel] [varchar](500) NULL,
	[OtherTel] [varchar](500) NULL,
	[Fax] [varchar](50) NULL,
	[Email] [varchar](500) NULL,
	[QQ] [varchar](50) NULL,
	[HeadImg] [varchar](500) NULL,
	[WeiXin] [varchar](50) NULL,
	[PartTimeId] [uniqueidentifier] NULL,
	[Office] [nvarchar](50) NULL,
	[WeiXinOpenId] [varchar](200) NULL,
 CONSTRAINT [PK_RF_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [RF_User_Account_Index] ON [dbo].[RF_User] 
(
	[Account] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帐号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Account'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Password'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'性别 0男 1女' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Sex'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态 0 正常 1 冻结' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'职务' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Job'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'手机' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Mobile'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'办公电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Tel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'其它联系方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'OtherTel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'传真' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Fax'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮箱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Email'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'QQ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'QQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'头像' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'HeadImg'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'微信号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'WeiXin'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'人员兼职的机构ID（兼职时有用,数据表为空，在获取人员时判断）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'PartTimeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'科室' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'Office'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'微信openid' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_User', @level2type=N'COLUMN',@level2name=N'WeiXinOpenId'
GO
/****** Object:  Table [dbo].[RF_TestSub]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_TestSub](
	[Id] [uniqueidentifier] NOT NULL,
	[TestId] [uniqueidentifier] NOT NULL,
	[f1] [varchar](500) NULL,
	[f2] [varchar](500) NULL,
	[f3] [varchar](500) NULL,
	[f4] [varchar](500) NULL,
	[f5] [varchar](500) NULL,
	[f6] [varchar](500) NULL,
	[f7] [varchar](500) NULL,
	[f8] [varchar](500) NULL,
	[f9] [varchar](500) NULL,
	[f10] [varchar](500) NULL,
	[f11] [varchar](5000) NULL,
 CONSTRAINT [PK_RF_TestSub] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RF_Test1Sub]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Test1Sub](
	[TestId] [int] NOT NULL,
	[f1] [varchar](50) NULL,
	[f2] [varchar](5000) NULL,
	[f3] [decimal](18, 2) NULL,
	[f4] [int] NULL,
	[f5] [decimal](18, 2) NULL,
	[f6] [varchar](5000) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RF_Test1]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Test1](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](5000) NOT NULL,
	[f1] [varchar](5000) NOT NULL,
	[f2] [varchar](5000) NOT NULL,
	[f3] [varchar](5000) NULL,
	[f4] [varchar](5000) NULL,
 CONSTRAINT [PK_RF_Test1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RF_Test]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Test](
	[Id] [uniqueidentifier] NOT NULL,
	[f1] [varchar](max) NULL,
	[f2] [varchar](max) NULL,
	[f3] [varchar](max) NULL,
	[f4] [varchar](max) NULL,
	[f5] [varchar](max) NULL,
	[f6] [varchar](max) NULL,
	[f7] [varchar](max) NULL,
	[f8] [varchar](max) NULL,
	[f9] [varchar](max) NULL,
	[f10] [varchar](max) NULL,
	[f11] [datetime] NULL,
	[f12] [varchar](max) NULL,
	[f13] [varchar](max) NULL,
	[f14] [varchar](max) NULL,
	[f15] [varchar](max) NULL,
	[f16] [int] NULL,
	[f18] [varchar](max) NULL,
	[f19] [varchar](max) NULL,
	[f20] [varchar](max) NULL,
	[f21] [varchar](max) NULL,
	[f22] [varchar](max) NULL,
	[f23] [varchar](max) NULL,
	[f24] [datetime] NULL,
 CONSTRAINT [PK_RF_Test] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RF_SystemField]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_SystemField](
	[Id] [uniqueidentifier] NOT NULL,
	[SystemTable] [varchar](50) NULL,
	[TableId] [int] NULL,
	[FieldName] [varchar](50) NULL,
	[FieldDescribe] [varchar](100) NULL,
	[FieldType] [varchar](50) NULL,
	[FieldDecimalDigits] [varchar](50) NULL,
	[FieldLength] [int] NULL,
	[FieldIdentity] [varchar](50) NULL,
	[FieldPrimaryKey] [varchar](50) NULL,
	[FieldAllowEmpty] [varchar](50) NULL,
	[FieldDefaultValue] [varchar](50) NULL,
	[CreateDate] [datetime] NULL,
 CONSTRAINT [PK_RF_SystemField] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统字段Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统表名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'SystemTable'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表名序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'TableId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'列名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'FieldName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'列说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'FieldDescribe'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'FieldType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'小数位数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'FieldDecimalDigits'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'长度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'FieldLength'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'FieldIdentity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'FieldPrimaryKey'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'允许空' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'FieldAllowEmpty'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'默认值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'FieldDefaultValue'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemField', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
/****** Object:  Table [dbo].[RF_SystemButton]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_SystemButton](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Events] [varchar](4000) NOT NULL,
	[Ico] [varchar](200) NULL,
	[Note] [nvarchar](200) NULL,
	[Sort] [int] NOT NULL,
	[Name_en] [varchar](100) NULL,
	[Name_zh] [nvarchar](50) NULL,
 CONSTRAINT [PK_RF_SystemButton] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统按钮Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemButton', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemButton', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'脚本' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemButton', @level2type=N'COLUMN',@level2name=N'Events'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemButton', @level2type=N'COLUMN',@level2name=N'Ico'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemButton', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemButton', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'英语名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemButton', @level2type=N'COLUMN',@level2name=N'Name_en'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'繁体中文名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_SystemButton', @level2type=N'COLUMN',@level2name=N'Name_zh'
GO
/****** Object:  Table [dbo].[RF_ProgramValidate]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_ProgramValidate](
	[Id] [uniqueidentifier] NOT NULL,
	[ProgramId] [uniqueidentifier] NOT NULL,
	[TableName] [varchar](500) NOT NULL,
	[FieldName] [varchar](500) NOT NULL,
	[FieldNote] [nvarchar](500) NULL,
	[Validate] [int] NOT NULL,
	[Status] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序表单验证Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramValidate', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramValidate', @level2type=N'COLUMN',@level2name=N'ProgramId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramValidate', @level2type=N'COLUMN',@level2name=N'TableName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramValidate', @level2type=N'COLUMN',@level2name=N'FieldName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramValidate', @level2type=N'COLUMN',@level2name=N'FieldNote'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'验证类型 0不检查 1允许为空,非空时检查 2不允许为空,并检查' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramValidate', @level2type=N'COLUMN',@level2name=N'Validate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态 0编辑 1只读 2隐藏' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramValidate', @level2type=N'COLUMN',@level2name=N'Status'
GO
/****** Object:  Table [dbo].[RF_ProgramQuery]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_ProgramQuery](
	[Id] [uniqueidentifier] NOT NULL,
	[ProgramId] [uniqueidentifier] NOT NULL,
	[Field] [varchar](500) NOT NULL,
	[ShowTitle] [nvarchar](500) NULL,
	[Operators] [varchar](50) NOT NULL,
	[ControlName] [varchar](50) NULL,
	[InputType] [int] NOT NULL,
	[ShowStyle] [varchar](200) NULL,
	[Sort] [int] NOT NULL,
	[DataSource] [int] NULL,
	[DataSourceString] [varchar](max) NULL,
	[DictValue] [varchar](50) NULL,
	[ConnId] [varchar](50) NULL,
	[IsQueryUsers] [int] NULL,
	[OrgAttribute] [varchar](500) NULL,
 CONSTRAINT [PK_RF_ProgramQuery] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序查询Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'ProgramId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'Field'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'ShowTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作符' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'Operators'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'控件名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'ControlName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'输入类型 0文本 1日期 2日期范围 3日期时间 4日期时间范围 5下拉选择 6组织机构 7数据字典' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'InputType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示样式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'ShowStyle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示顺序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据来源 0.字符串表达式 1.数据字典 2.SQL' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'DataSource'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据连接字符串' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'DataSourceString'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据来源为数据字典时的值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'DictValue'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据来源为SQL时的数据连接ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'ConnId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织机构查询时是否将选择转换为人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'IsQueryUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当输入类型为组织机构时的属性' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramQuery', @level2type=N'COLUMN',@level2name=N'OrgAttribute'
GO
/****** Object:  Table [dbo].[RF_ProgramGroup]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_ProgramGroup](
	[Id] [uniqueidentifier] NOT NULL,
	[ProgramId] [uniqueidentifier] NOT NULL,
	[GroupField] [varchar](500) NULL,
	[IsGroupSummary] [int] NOT NULL,
	[IsGroupColumnShow] [int] NOT NULL,
	[GroupText] [varchar](500) NULL,
	[IsGroupCollapse] [int] NOT NULL,
	[GroupOrder] [varchar](500) NULL,
	[IsGroupDataSorted] [int] NOT NULL,
	[IsShowSummaryOnHide] [int] NOT NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_ProgramGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序设计成组Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序设计Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'ProgramId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分组字段名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'GroupField'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否显示汇总' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'IsGroupSummary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否显示分组列' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'IsGroupColumnShow'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分组表头行设置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'GroupText'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否折叠分组' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'IsGroupCollapse'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分组排序方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'GroupOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分组中的数据是否排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'IsGroupDataSorted'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示或隐藏汇总行' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'IsShowSummaryOnHide'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分组排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramGroup', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_ProgramField]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_ProgramField](
	[Id] [uniqueidentifier] NOT NULL,
	[ProgramId] [uniqueidentifier] NOT NULL,
	[Field] [varchar](500) NULL,
	[ShowTitle] [varchar](500) NULL,
	[Align] [varchar](50) NOT NULL,
	[Width] [varchar](50) NULL,
	[ShowType] [int] NOT NULL,
	[ShowFormat] [varchar](50) NULL,
	[CustomString] [varchar](max) NULL,
	[IsSort] [varchar](500) NULL,
	[IsDefaultSort] [int] NOT NULL,
	[Sort] [int] NOT NULL,
	[SummaryType] [nvarchar](50) NULL,
	[SummaryTpl] [nvarchar](50) NULL,
	[Formatter] [nvarchar](50) NULL,
	[FormatOptions] [nvarchar](2000) NULL,
	[IsFreeze] [int] NULL,
	[SortWay] [nvarchar](50) NULL,
 CONSTRAINT [PK_RF_ProgramField] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序字段Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'ProgramId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'Field'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'ShowTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'对齐方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'Align'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'宽度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'Width'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0直接输出 1序号 2日期时间 3数字 4数据字典ID显示标题  5组织机构id显示为名称 6自定义' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'ShowType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'格式化字符串' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'ShowFormat'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'自定义字符串' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'CustomString'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否可以排序(jqgrid点击列排序)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'IsSort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否默认排序列' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'IsDefaultSort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运算类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'SummaryType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运算显示方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'SummaryTpl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'格式化' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'Formatter'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'格式化选项' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'FormatOptions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否冻结' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'IsFreeze'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramField', @level2type=N'COLUMN',@level2name=N'SortWay'
GO
/****** Object:  Table [dbo].[RF_ProgramExport]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_ProgramExport](
	[Id] [uniqueidentifier] NOT NULL,
	[ProgramId] [uniqueidentifier] NOT NULL,
	[Field] [varchar](500) NOT NULL,
	[ShowTitle] [nvarchar](500) NULL,
	[Align] [varchar](50) NOT NULL,
	[Width] [int] NULL,
	[ShowType] [int] NULL,
	[DataType] [int] NULL,
	[ShowFormat] [varchar](50) NULL,
	[CustomString] [varchar](5000) NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_ProgramExport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序导出Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'ProgramId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字段' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'Field'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示列名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'ShowTitle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'对齐方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'Align'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'列宽度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'Width'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示类型 0直接输出 1序号 2日期时间 3数字 4数据字典ID显示标题  5组织机构id显示为名称 6自定义' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'ShowType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单元格类型：0常规 1文本 2数字 3日期时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'DataType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'格式化字符串' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'ShowFormat'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'自定义字符串' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'CustomString'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示顺序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramExport', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_ProgramButton]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_ProgramButton](
	[Id] [uniqueidentifier] NOT NULL,
	[ProgramId] [uniqueidentifier] NOT NULL,
	[ButtonId] [uniqueidentifier] NULL,
	[ButtonName] [nvarchar](200) NOT NULL,
	[ClientScript] [varchar](max) NULL,
	[Ico] [varchar](500) NULL,
	[ShowType] [int] NOT NULL,
	[Sort] [int] NOT NULL,
	[IsValidateShow] [int] NOT NULL,
 CONSTRAINT [PK_RF_ProgramButton] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序按钮Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramButton', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramButton', @level2type=N'COLUMN',@level2name=N'ProgramId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统按钮库ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramButton', @level2type=N'COLUMN',@level2name=N'ButtonId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramButton', @level2type=N'COLUMN',@level2name=N'ButtonName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'脚本' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramButton', @level2type=N'COLUMN',@level2name=N'ClientScript'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramButton', @level2type=N'COLUMN',@level2name=N'Ico'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示类型 0工具栏按钮 1普通按钮 2列表按键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramButton', @level2type=N'COLUMN',@level2name=N'ShowType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramButton', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否验证权限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_ProgramButton', @level2type=N'COLUMN',@level2name=N'IsValidateShow'
GO
/****** Object:  Table [dbo].[RF_Program]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Program](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[Type] [uniqueidentifier] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[PublishTime] [datetime] NULL,
	[CreateUserId] [uniqueidentifier] NOT NULL,
	[SqlString] [varchar](max) NOT NULL,
	[IsAdd] [int] NOT NULL,
	[ConnId] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL,
	[FormId] [varchar](50) NULL,
	[EditModel] [int] NOT NULL,
	[Width] [varchar](50) NULL,
	[Height] [varchar](50) NULL,
	[ButtonLocation] [int] NOT NULL,
	[IsPager] [int] NOT NULL,
	[SelectColumn] [int] NOT NULL,
	[RowNumber] [int] NOT NULL,
	[ClientScript] [varchar](max) NULL,
	[ExportTemplate] [varchar](200) NULL,
	[ExportHeaderText] [nvarchar](500) NULL,
	[ExportFileName] [varchar](200) NULL,
	[TableStyle] [varchar](200) NULL,
	[TableHead] [varchar](max) NULL,
	[InDataNumberFiledName] [varchar](500) NULL,
	[GroupHeaders] [varchar](max) NULL,
	[IsGroup] [int] NOT NULL,
	[IsFooterrow] [int] NULL,
	[DefaultSort] [varchar](500) NULL,
 CONSTRAINT [PK_RF_Program] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'程序Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'应用名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分类' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'CreateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发布时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'PublishTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'查询SQL' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'SqlString'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否显示新增按钮' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'IsAdd'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据连接ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'ConnId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态 0设计中 1已发布 2已作废' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'FormId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编辑模式 0，当前窗口 1，弹出层 2，弹出窗口' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'EditModel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'弹出层宽度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'Width'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'弹出层高度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'Height'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'按钮显示位置 0新行 1查询后面' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'ButtonLocation'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否分页' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'IsPager'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'选择列 0无 1单选 2多选' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'SelectColumn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否显示序号列' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'RowNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'页面脚本' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'ClientScript'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导出EXCEL模板' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'ExportTemplate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导出Excel表头' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'ExportHeaderText'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导出EXCLE的文件名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'ExportFileName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'列表样式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'TableStyle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'列表表头HTML' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'TableHead'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'导入EXCEL数据时的标识字段，每次导入生成一个编号区分' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'InDataNumberFiledName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表头合并' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'GroupHeaders'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否分组' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'IsGroup'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否汇总合计' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'IsFooterrow'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'默认排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Program', @level2type=N'COLUMN',@level2name=N'DefaultSort'
GO
/****** Object:  Table [dbo].[RF_OrganizeUser]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RF_OrganizeUser](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizeId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IsMain] [int] NOT NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_OrganizeUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织结构用户Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_OrganizeUser', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织机构Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_OrganizeUser', @level2type=N'COLUMN',@level2name=N'OrganizeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'人员Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_OrganizeUser', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否主要' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_OrganizeUser', @level2type=N'COLUMN',@level2name=N'IsMain'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_OrganizeUser', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_Organize]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Organize](
	[Id] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Type] [int] NOT NULL,
	[Leader] [varchar](200) NULL,
	[ChargeLeader] [varchar](200) NULL,
	[Note] [varchar](200) NULL,
	[Status] [int] NOT NULL,
	[Sort] [int] NOT NULL,
	[IntId] [int] NOT NULL,
	[Office] [nvarchar](50) NULL,
 CONSTRAINT [PK_RF_Organize] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组织结构Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'ParentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型:1 单位 2 部门 3 岗位' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门或岗位领导' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'Leader'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分管领导' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'ChargeLeader'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态  0 正常 1 冻结' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'这里为了其他系统调用，比如微信' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'IntId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'科室' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Organize', @level2type=N'COLUMN',@level2name=N'Office'
GO
/****** Object:  Table [dbo].[RF_MessageUser]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RF_MessageUser](
	[MessageId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IsRead] [int] NOT NULL,
	[ReadTime] [datetime] NULL,
 CONSTRAINT [PK_RF_MessageUser_1] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'消息用户Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MessageUser', @level2type=N'COLUMN',@level2name=N'MessageId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MessageUser', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否读取' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MessageUser', @level2type=N'COLUMN',@level2name=N'IsRead'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'读取时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MessageUser', @level2type=N'COLUMN',@level2name=N'ReadTime'
GO
/****** Object:  Table [dbo].[RF_Message]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Message](
	[Id] [uniqueidentifier] NOT NULL,
	[Contents] [varchar](900) NOT NULL,
	[SendType] [varchar](50) NOT NULL,
	[SiteMessage] [int] NOT NULL,
	[SenderId] [uniqueidentifier] NULL,
	[SenderName] [nvarchar](50) NULL,
	[ReceiverIdString] [varchar](max) NOT NULL,
	[SendTime] [datetime] NOT NULL,
	[Type] [int] NOT NULL,
	[Files] [varchar](max) NULL,
 CONSTRAINT [PK_RF_Message] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_RF_Message] ON [dbo].[RF_Message] 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'消息Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'消息内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'Contents'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送方式 0站内消息 1手机短信 2微信 ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'SendType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否是站内短信（把发送类型分开是为了提高查询效率）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'SiteMessage'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'SenderId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'SenderName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收人组织机构字符串' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'ReceiverIdString'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'SendTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0:用户发送消息 1：系统消息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'附件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Message', @level2type=N'COLUMN',@level2name=N'Files'
GO
/****** Object:  Table [dbo].[RF_MenuUser]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_MenuUser](
	[Id] [uniqueidentifier] NOT NULL,
	[MenuId] [uniqueidentifier] NOT NULL,
	[Organizes] [varchar](50) NOT NULL,
	[Users] [varchar](max) NULL,
	[Buttons] [varchar](max) NULL,
	[Params] [varchar](max) NULL,
 CONSTRAINT [PK_RF_MenuUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'菜单用户Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MenuUser', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'菜单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MenuUser', @level2type=N'COLUMN',@level2name=N'MenuId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用对象（组织机构ID）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MenuUser', @level2type=N'COLUMN',@level2name=N'Organizes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用人员，人员ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MenuUser', @level2type=N'COLUMN',@level2name=N'Users'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'可使用的按钮' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MenuUser', @level2type=N'COLUMN',@level2name=N'Buttons'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MenuUser', @level2type=N'COLUMN',@level2name=N'Params'
GO
/****** Object:  Table [dbo].[RF_Menu]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Menu](
	[Id] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NOT NULL,
	[AppLibraryId] [uniqueidentifier] NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Params] [varchar](2000) NULL,
	[Ico] [varchar](200) NULL,
	[IcoColor] [varchar](50) NULL,
	[Sort] [int] NOT NULL,
	[Title_en] [varchar](200) NULL,
	[Title_zh] [nvarchar](200) NULL,
 CONSTRAINT [PK_RF_Menu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'菜单Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'ParentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'应用程序库Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'AppLibraryId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'菜单名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'URL参数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'Params'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'Ico'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图标颜色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'IcoColor'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'英文标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'Title_en'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'繁体中文标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Menu', @level2type=N'COLUMN',@level2name=N'Title_zh'
GO
/****** Object:  Table [dbo].[RF_MailOutBox]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_MailOutBox](
	[Id] [uniqueidentifier] NOT NULL,
	[Subject] [varchar](500) NOT NULL,
	[SubjectColor] [varchar](50) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ReceiveUsers] [varchar](max) NOT NULL,
	[SendDateTime] [datetime] NOT NULL,
	[ContentsId] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL,
	[CarbonCopy] [varchar](max) NULL,
	[SecretCopy] [varchar](max) NULL,
 CONSTRAINT [PK_RF_MailOutBox] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发信息Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'Subject'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主题颜色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'SubjectColor'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收人员(组织机构ID字符串)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'ReceiveUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'SendDateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮件内容ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'ContentsId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0 草稿 1已发送' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'抄送' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'CarbonCopy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密送' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailOutBox', @level2type=N'COLUMN',@level2name=N'SecretCopy'
GO
/****** Object:  Table [dbo].[RF_MailInBox]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_MailInBox](
	[Id] [uniqueidentifier] NOT NULL,
	[Subject] [varchar](500) NOT NULL,
	[SubjectColor] [varchar](50) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[SendUserId] [uniqueidentifier] NOT NULL,
	[SendDateTime] [datetime] NOT NULL,
	[ContentsId] [uniqueidentifier] NOT NULL,
	[IsRead] [int] NOT NULL,
	[ReadDateTime] [datetime] NULL,
	[OutBoxId] [uniqueidentifier] NOT NULL,
	[MailType] [int] NOT NULL,
 CONSTRAINT [PK_RF_MailInBox] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收信息Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'Subject'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主题颜色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'SubjectColor'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'SendUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'SendDateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮件内容Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'ContentsId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否查看' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'IsRead'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'查看时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'ReadDateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发件ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'OutBoxId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮件类型 1发送 2抄送 3密送' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailInBox', @level2type=N'COLUMN',@level2name=N'MailType'
GO
/****** Object:  Table [dbo].[RF_MailDeletedBox]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_MailDeletedBox](
	[Id] [uniqueidentifier] NOT NULL,
	[Subject] [varchar](500) NOT NULL,
	[SubjectColor] [varchar](50) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[SendUserId] [uniqueidentifier] NOT NULL,
	[SendDateTime] [datetime] NOT NULL,
	[ContentsId] [uniqueidentifier] NOT NULL,
	[IsRead] [int] NOT NULL,
	[ReadDateTime] [datetime] NULL,
	[OutBoxId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_RF_MailDeletedBox] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮箱删除Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'Subject'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主题颜色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'SubjectColor'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'SendUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'SendDateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'内容ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'ContentsId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否查看' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'IsRead'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'查看时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'ReadDateTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发件ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailDeletedBox', @level2type=N'COLUMN',@level2name=N'OutBoxId'
GO
/****** Object:  Table [dbo].[RF_MailContent]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_MailContent](
	[Id] [uniqueidentifier] NOT NULL,
	[Contents] [text] NOT NULL,
	[Files] [varchar](max) NULL,
 CONSTRAINT [PK_RF_MailContent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮件内容Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailContent', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮件内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailContent', @level2type=N'COLUMN',@level2name=N'Contents'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'附件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_MailContent', @level2type=N'COLUMN',@level2name=N'Files'
GO
/****** Object:  Table [dbo].[RF_Log]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Log](
	[Id] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](900) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[WriteTime] [datetime] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[UserName] [nvarchar](50) NULL,
	[IPAddress] [varchar](50) NULL,
	[Referer] [varchar](max) NULL,
	[URL] [varchar](max) NULL,
	[Contents] [varchar](max) NULL,
	[Others] [varchar](max) NULL,
	[NewContents] [varchar](max) NULL,
	[OldContents] [varchar](max) NULL,
	[BrowseAgent] [varchar](max) NULL,
	[CityAddress] [nvarchar](50) NULL,
 CONSTRAINT [PK_RF_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'写入时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'WriteTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'IP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'IPAddress'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'来源URL' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'Referer'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发生URL' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'URL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'Contents'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'其它' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'Others'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更改后' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'NewContents'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更改前' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'OldContents'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'浏览器信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'BrowseAgent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'城市地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Log', @level2type=N'COLUMN',@level2name=N'CityAddress'
GO
/****** Object:  Table [dbo].[RF_HomeSet]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_HomeSet](
	[Id] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[DataSourceType] [int] NOT NULL,
	[DataSource] [varchar](max) NULL,
	[Ico] [varchar](50) NULL,
	[BackgroundColor] [varchar](50) NULL,
	[FontColor] [varchar](50) NULL,
	[DbConnId] [uniqueidentifier] NULL,
	[LinkURL] [varchar](2000) NULL,
	[UseOrganizes] [varchar](max) NULL,
	[UseUsers] [varchar](max) NULL,
	[Sort] [int] NOT NULL,
	[Note] [nvarchar](200) NULL,
 CONSTRAINT [PK_RF_HomeSet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'首页设置Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'模块类型 0上方信息提示模块 1左边模块 2右边模块' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据来源 0:sql,1:c#方法 2:url' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'DataSourceType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据源' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'DataSource'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'Ico'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'背景色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'BackgroundColor'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字体色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'FontColor'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据连接ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'DbConnId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'连接地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'LinkURL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用对象' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'UseOrganizes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'UseUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_HomeSet', @level2type=N'COLUMN',@level2name=N'Note'
GO
/****** Object:  Table [dbo].[RF_Form]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Form](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[FormType] [uniqueidentifier] NOT NULL,
	[CreateUserId] [uniqueidentifier] NOT NULL,
	[CreateUserName] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Html] [text] NULL,
	[SubtableJSON] [text] NULL,
	[EventJSON] [text] NULL,
	[attribute] [varchar](4000) NULL,
	[Status] [int] NOT NULL,
	[Note] [nvarchar](200) NULL,
	[RunHtml] [text] NULL,
	[ManageUser] [varchar](2000) NOT NULL,
 CONSTRAINT [PK_RF_Form] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表单名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表单分类' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'FormType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人员ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'CreateUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人员姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'CreateUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'EditDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表单HTML' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'Html'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子表json' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'SubtableJSON'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'事件json' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'EventJSON'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'属性json' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'attribute'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态：0 保存 1 编译 2作废' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'生成后的HTML' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'RunHtml'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'管理人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Form', @level2type=N'COLUMN',@level2name=N'ManageUser'
GO
/****** Object:  Table [dbo].[RF_FlowTask]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_FlowTask](
	[Id] [uniqueidentifier] NOT NULL,
	[PrevId] [uniqueidentifier] NOT NULL,
	[PrevStepId] [uniqueidentifier] NOT NULL,
	[FlowId] [uniqueidentifier] NOT NULL,
	[FlowName] [nvarchar](200) NOT NULL,
	[StepId] [uniqueidentifier] NOT NULL,
	[StepName] [nvarchar](200) NOT NULL,
	[InstanceId] [varchar](200) NULL,
	[GroupId] [uniqueidentifier] NOT NULL,
	[TaskType] [int] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[SenderId] [uniqueidentifier] NOT NULL,
	[SenderName] [nvarchar](50) NOT NULL,
	[ReceiveId] [uniqueidentifier] NOT NULL,
	[ReceiveName] [nvarchar](50) NOT NULL,
	[ReceiveTime] [datetime] NOT NULL,
	[OpenTime] [datetime] NULL,
	[CompletedTime] [datetime] NULL,
	[CompletedTime1] [datetime] NULL,
	[Comments] [nvarchar](200) NULL,
	[IsSign] [int] NOT NULL,
	[Note] [nvarchar](200) NULL,
	[SubFlowGroupId] [varchar](2000) NULL,
	[IsAutoSubmit] [int] NOT NULL,
	[Attachment] [varchar](2000) NULL,
	[Status] [int] NOT NULL,
	[Sort] [int] NOT NULL,
	[ExecuteType] [int] NOT NULL,
	[ReceiveOrganizeId] [uniqueidentifier] NULL,
	[StepSort] [int] NOT NULL,
	[EntrustUserId] [uniqueidentifier] NULL,
	[OtherType] [int] NOT NULL,
	[NextStepsHandle] [varchar](max) NULL,
	[BeforeStepId] [uniqueidentifier] NULL,
	[RemindTime] [datetime] NULL,
	[IsBatch] [int] NULL,
 CONSTRAINT [PK_RF_FlowTask] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [RF_INDEX_FLOWTASK_RECEIVETIME] ON [dbo].[RF_FlowTask] 
(
	[ReceiveTime] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [RF_INDEX_GroupId] ON [dbo].[RF_FlowTask] 
(
	[GroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'任务ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上一任务ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'PrevId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上一步骤ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'PrevStepId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'FlowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'FlowName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'步骤ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'StepId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'步骤名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'StepName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'对应业务表主键值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'InstanceId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分组ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'GroupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'任务类型 0常规 1指派 2委托 3转交 4退回 5抄送 6前加签 7并签 8后加签' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'TaskType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'任务标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送人ID(如果是兼职岗位R_关系表ID)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'SenderId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发送人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'SenderName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收人ID(如果是兼职岗位R_关系表ID)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'ReceiveId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'ReceiveName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'ReceiveTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打开时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'OpenTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'要求完成时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'CompletedTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实际完成时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'CompletedTime1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'处理意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'Comments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否签章' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'IsSign'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子流程实例分组ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'SubFlowGroupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否超时自动提交 0否 1是' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'IsAutoSubmit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'附件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'Attachment'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'任务状态 -1等待中 0未处理 1处理中 2已完成' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'任务顺序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'处理类型 -1等待中 0未处理 1处理中 2已完成 3已退回 4他人已处理 5他人已退回 6已转交 7已委托 8已阅知' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'ExecuteType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收人所在机构ID（如果是兼职人员的情况下这里有值）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'ReceiveOrganizeId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'一个步骤内的处理顺序(选择人员顺序处理时的处理顺序)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'StepSort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'如果是委托任务，这里记录委托人员ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'EntrustUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'其它类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'OtherType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'指定的后续步骤处理人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'NextStepsHandle'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原步骤ID(动态步骤的原步骤ID)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'BeforeStepId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'提醒时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'RemindTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否可以批量提交' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowTask', @level2type=N'COLUMN',@level2name=N'IsBatch'
GO
/****** Object:  Table [dbo].[RF_FlowEntrust]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_FlowEntrust](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[FlowId] [uniqueidentifier] NULL,
	[ToUserId] [varchar](50) NOT NULL,
	[WriteTime] [datetime] NOT NULL,
	[Note] [nvarchar](200) NULL,
 CONSTRAINT [PK_RF_FlowEntrust] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程委托Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowEntrust', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'委托人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowEntrust', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开始时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowEntrust', @level2type=N'COLUMN',@level2name=N'StartTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'结束时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowEntrust', @level2type=N'COLUMN',@level2name=N'EndTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'委托流程ID,为空表示所有流程' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowEntrust', @level2type=N'COLUMN',@level2name=N'FlowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'被委托人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowEntrust', @level2type=N'COLUMN',@level2name=N'ToUserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'设置时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowEntrust', @level2type=N'COLUMN',@level2name=N'WriteTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowEntrust', @level2type=N'COLUMN',@level2name=N'Note'
GO
/****** Object:  Table [dbo].[RF_FlowDynamic]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_FlowDynamic](
	[StepId] [uniqueidentifier] NOT NULL,
	[GroupId] [uniqueidentifier] NOT NULL,
	[FlowJSON] [varchar](max) NOT NULL,
 CONSTRAINT [PK_RF_FlowDynamic] PRIMARY KEY CLUSTERED 
(
	[StepId] ASC,
	[GroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'动态步骤ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowDynamic', @level2type=N'COLUMN',@level2name=N'StepId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowDynamic', @level2type=N'COLUMN',@level2name=N'GroupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程JSON' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowDynamic', @level2type=N'COLUMN',@level2name=N'FlowJSON'
GO
/****** Object:  Table [dbo].[RF_FlowComment]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RF_FlowComment](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[AddType] [int] NOT NULL,
	[Comments] [nvarchar](200) NOT NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_FlowComment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程评论Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowComment', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'意见使用人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowComment', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型 0用户添加 1管理员添加' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowComment', @level2type=N'COLUMN',@level2name=N'AddType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowComment', @level2type=N'COLUMN',@level2name=N'Comments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowComment', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_FlowButton]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_FlowButton](
	[Id] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](20) NOT NULL,
	[Ico] [varchar](200) NULL,
	[Script] [varchar](2000) NULL,
	[Note] [nvarchar](100) NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_FlowButton] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程按钮Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowButton', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'按钮标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowButton', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'按钮图标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowButton', @level2type=N'COLUMN',@level2name=N'Ico'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'按钮脚本' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowButton', @level2type=N'COLUMN',@level2name=N'Script'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowButton', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowButton', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_FlowArchive]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_FlowArchive](
	[Id] [uniqueidentifier] NOT NULL,
	[FlowId] [uniqueidentifier] NOT NULL,
	[StepId] [uniqueidentifier] NOT NULL,
	[FlowName] [nvarchar](200) NOT NULL,
	[StepName] [nvarchar](200) NOT NULL,
	[TaskId] [uniqueidentifier] NOT NULL,
	[GroupId] [uniqueidentifier] NOT NULL,
	[InstanceId] [varchar](200) NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[UserId] [varchar](50) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[DataJson] [varchar](max) NOT NULL,
	[Comments] [varchar](max) NULL,
	[WriteTime] [datetime] NOT NULL,
	[FormHtml] [text] NULL,
 CONSTRAINT [PK_RF_FlowArchive] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程存档Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'FlowId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'步骤' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'StepId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'FlowName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'步骤名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'StepName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'任务ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'TaskId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'组' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'GroupId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实例ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'InstanceId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'处理人ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'处理人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'DataJson'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'处理意见HTML' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'Comments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'写入时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'WriteTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'表单HTML' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowArchive', @level2type=N'COLUMN',@level2name=N'FormHtml'
GO
/****** Object:  Table [dbo].[RF_FlowApiSystem]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_FlowApiSystem](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[SystemCode] [varchar](50) NOT NULL,
	[SystemIP] [varchar](4000) NOT NULL,
	[Note] [nvarchar](500) NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_FlowApiSystem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程Api系统Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowApiSystem', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowApiSystem', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统标识(不能重复)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowApiSystem', @level2type=N'COLUMN',@level2name=N'SystemCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调用IP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowApiSystem', @level2type=N'COLUMN',@level2name=N'SystemIP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowApiSystem', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowApiSystem', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'说明', @value=N'调用流程的系统（API接口模式）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_FlowApiSystem'
GO
/****** Object:  Table [dbo].[RF_Flow]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Flow](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[FlowType] [uniqueidentifier] NOT NULL,
	[Manager] [varchar](2000) NOT NULL,
	[InstanceManager] [varchar](2000) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [uniqueidentifier] NOT NULL,
	[DesignerJSON] [text] NULL,
	[RunJSON] [text] NULL,
	[InstallDate] [datetime] NULL,
	[InstallUser] [uniqueidentifier] NULL,
	[Status] [int] NOT NULL,
	[Note] [nvarchar](200) NULL,
	[SystemId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_RF_Flow] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流程Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分类' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'FlowType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'管理人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'Manager'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实例管理人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'InstanceManager'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'CreateUser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'设计时JSON' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'DesignerJSON'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运行时JSON' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'RunJSON'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'安装日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'InstallDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'安装人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'InstallUser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态 0设计中 1已安装 2已卸载 3已删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属系统Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Flow', @level2type=N'COLUMN',@level2name=N'SystemId'
GO
/****** Object:  Table [dbo].[RF_DocUser]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RF_DocUser](
	[DocId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IsRead] [int] NOT NULL,
	[ReadTime] [datetime] NULL,
 CONSTRAINT [PK_RF_DocUser_1] PRIMARY KEY CLUSTERED 
(
	[DocId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [INDEX_RF_DocUser_DocID_UserId] ON [dbo].[RF_DocUser] 
(
	[DocId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文档id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocUser', @level2type=N'COLUMN',@level2name=N'DocId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'人员id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocUser', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否已读' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocUser', @level2type=N'COLUMN',@level2name=N'IsRead'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'阅读时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocUser', @level2type=N'COLUMN',@level2name=N'ReadTime'
GO
/****** Object:  Table [dbo].[RF_DocDir]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_DocDir](
	[Id] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[ReadUsers] [varchar](max) NULL,
	[ManageUsers] [varchar](max) NULL,
	[PublishUsers] [varchar](max) NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_DocDir] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件栏目Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocDir', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocDir', @level2type=N'COLUMN',@level2name=N'ParentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'栏目名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocDir', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'阅读人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocDir', @level2type=N'COLUMN',@level2name=N'ReadUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'管理人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocDir', @level2type=N'COLUMN',@level2name=N'ManageUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'发布人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocDir', @level2type=N'COLUMN',@level2name=N'PublishUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DocDir', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_Doc]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Doc](
	[Id] [uniqueidentifier] NOT NULL,
	[DirId] [uniqueidentifier] NOT NULL,
	[DirName] [nvarchar](200) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Source] [nvarchar](50) NULL,
	[Contents] [varchar](max) NOT NULL,
	[Files] [varchar](max) NULL,
	[WriteTime] [datetime] NOT NULL,
	[WriteUserID] [uniqueidentifier] NOT NULL,
	[WriteUserName] [nvarchar](50) NOT NULL,
	[EditTime] [datetime] NULL,
	[EditUserID] [uniqueidentifier] NULL,
	[EditUserName] [nvarchar](50) NULL,
	[ReadUsers] [varchar](max) NULL,
	[ReadCount] [int] NOT NULL,
	[DocRank] [int] NOT NULL,
 CONSTRAINT [PK_RF_Doc] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [INDEX_RF_Doc_Title] ON [dbo].[RF_Doc] 
(
	[Title] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文件Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'栏目Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'DirId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'栏目名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'DirName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'来源' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'Source'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'Contents'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'相关附件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'Files'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'添加时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'WriteTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'添加人员Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'WriteUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'添加人员姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'WriteUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'EditTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改用户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'EditUserID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'EditUserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'阅读人员' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'ReadUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'阅读次数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'ReadCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'文档等级 0普通 1重要 2非常重要' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Doc', @level2type=N'COLUMN',@level2name=N'DocRank'
GO
/****** Object:  Table [dbo].[RF_Dictionary]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_Dictionary](
	[Id] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Code] [varchar](500) NULL,
	[Value] [varchar](2000) NULL,
	[Note] [varchar](2000) NULL,
	[Other] [varchar](2000) NULL,
	[Sort] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Title_en] [varchar](500) NULL,
	[Title_zh] [nvarchar](500) NULL,
 CONSTRAINT [PK_RF_Dictionary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'字典Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上级ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'ParentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一代码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Value'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'其它信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Other'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0 正常 1 删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题_英语' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Title_en'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题_繁体中文' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_Dictionary', @level2type=N'COLUMN',@level2name=N'Title_zh'
GO
/****** Object:  Table [dbo].[RF_DbConnection]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_DbConnection](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ConnType] [varchar](50) NOT NULL,
	[ConnString] [varchar](255) NOT NULL,
	[Note] [nvarchar](200) NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_RF_DbConnection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数据库连接Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DbConnection', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'连接名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DbConnection', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'连接类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DbConnection', @level2type=N'COLUMN',@level2name=N'ConnType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'连接字符串' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DbConnection', @level2type=N'COLUMN',@level2name=N'ConnString'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DbConnection', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_DbConnection', @level2type=N'COLUMN',@level2name=N'Sort'
GO
/****** Object:  Table [dbo].[RF_AppLibraryButton]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_AppLibraryButton](
	[Id] [uniqueidentifier] NOT NULL,
	[AppLibraryId] [uniqueidentifier] NOT NULL,
	[ButtonId] [uniqueidentifier] NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Events] [varchar](4000) NOT NULL,
	[Ico] [varchar](200) NULL,
	[Sort] [int] NOT NULL,
	[ShowType] [int] NOT NULL,
	[IsValidateShow] [int] NOT NULL,
 CONSTRAINT [PK_RF_AppLibraryButton] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'应用程序按钮Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibraryButton', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'应用程序库Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibraryButton', @level2type=N'COLUMN',@level2name=N'AppLibraryId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'按钮库按钮ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibraryButton', @level2type=N'COLUMN',@level2name=N'ButtonId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibraryButton', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'执行脚本' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibraryButton', @level2type=N'COLUMN',@level2name=N'Events'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibraryButton', @level2type=N'COLUMN',@level2name=N'Ico'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibraryButton', @level2type=N'COLUMN',@level2name=N'Sort'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示类型 0工具栏按钮 1普通按钮 2列表按键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibraryButton', @level2type=N'COLUMN',@level2name=N'ShowType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否验证权限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibraryButton', @level2type=N'COLUMN',@level2name=N'IsValidateShow'
GO
/****** Object:  Table [dbo].[RF_AppLibrary]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RF_AppLibrary](
	[Id] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Address] [varchar](200) NOT NULL,
	[Type] [uniqueidentifier] NULL,
	[OpenMode] [int] NOT NULL,
	[Width] [int] NULL,
	[Height] [int] NULL,
	[Note] [varchar](200) NULL,
	[Code] [varchar](50) NULL,
	[Title_en] [varchar](200) NULL,
	[Title_zh] [nvarchar](200) NULL,
 CONSTRAINT [PK_RF_AppLibrary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'应用程序库Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Address'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分类ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'打开方式{0-默认,1-弹出模态窗口,2-弹出窗口,3-新窗口}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'OpenMode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'弹出窗口宽度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Width'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'弹出窗口高度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Height'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一标识符，流程应用时为流程ID，表单应用时对应表单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题英语' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Title_en'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题繁体中文' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RF_AppLibrary', @level2type=N'COLUMN',@level2name=N'Title_zh'
GO
/****** Object:  Table [dbo].[N_Log]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[N_Log](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TraceId] [nvarchar](50) NOT NULL,
	[BusinessId] [nvarchar](50) NULL,
	[LogName] [nvarchar](100) NOT NULL,
	[Level] [nvarchar](20) NOT NULL,
	[OperationTime] [nvarchar](30) NOT NULL,
	[Duration] [nvarchar](50) NULL,
	[Url] [nvarchar](500) NULL,
	[Tenant] [nvarchar](100) NULL,
	[Application] [nvarchar](200) NULL,
	[Module] [nvarchar](500) NULL,
	[Class] [nvarchar](500) NULL,
	[Method] [nvarchar](200) NULL,
	[Params] [nvarchar](max) NULL,
	[Caption] [nvarchar](500) NULL,
	[Content] [nvarchar](max) NULL,
	[Sql] [nvarchar](max) NULL,
	[SqlParams] [nvarchar](max) NULL,
	[ErrorCode] [nvarchar](30) NULL,
	[ErrorMessage] [nvarchar](2000) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[UserId] [nvarchar](50) NULL,
	[Operator] [nvarchar](50) NULL,
	[Role] [nvarchar](500) NULL,
	[Ip] [nvarchar](50) NULL,
	[Host] [nvarchar](200) NULL,
	[ThreadId] [nvarchar](20) NULL,
	[Browser] [nvarchar](2000) NULL,
 CONSTRAINT [PK_LOG] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'跟踪号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'TraceId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'业务标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'BusinessId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'LogName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志级别' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Level'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'OperationTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'执行时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Duration'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'请求地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Url'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'租户' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Tenant'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'应用程序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Application'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'模块' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Module'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Class'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'方法' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Method'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'参数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Params'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Caption'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'内容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Content'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Sql语句' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Sql'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Sql参数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'SqlParams'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'错误码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'ErrorCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'错误消息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'ErrorMessage'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'堆栈跟踪' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'StackTrace'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作人标识' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'UserId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Operator'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'操作人角色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Role'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'IP地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Ip'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主机' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Host'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'线程号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'ThreadId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'浏览器' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'N_Log', @level2type=N'COLUMN',@level2name=N'Browser'
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertLog]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[SP_InsertLog](
    @traceId nvarchar(32),
    @eventId int,
    @user nvarchar(450),
    @application nvarchar(450),
    @level nvarchar(8),
    @category nvarchar(450),
    @message nvarchar(max),
    @properties nvarchar(max),
    @exception nvarchar(max),
    @clientIP nvarchar(450),
    @addTime datetime
)
as
begin
    insert into dbo.[LogTable]([TraceId],[EventId], [User], [Application], [Level], [Category], [Message], [Properties], [Exception], [ClientIP], [AddTime]) 
        values(@traceId, @eventId, @user, @application, @level, @category, @message, @properties, @exception, @clientIP, @addTime);
end
GO
/****** Object:  Table [dbo].[TryTestsub]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TryTestsub](
	[ID] [uniqueidentifier] NOT NULL,
	[s_name] [varchar](50) NULL,
	[s_address] [varchar](50) NULL,
	[s_sum] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TryTest1]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TryTest1](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[d_time] [datetime] NULL,
	[s_name] [varchar](50) NULL,
	[s_name1] [varchar](50) NULL,
	[s_name2] [varchar](50) NULL,
 CONSTRAINT [PK_TryTest1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TryTest]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TryTest](
	[ID] [uniqueidentifier] NOT NULL,
	[s_name] [varchar](200) NULL,
	[s_note] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TaskOptions_log]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskOptions_log](
	[n_log_id] [int] IDENTITY(1,1) NOT NULL,
	[s_log_type] [nvarchar](50) NULL,
	[d_log_time] [datetime] NOT NULL,
	[s_log_computer] [nvarchar](500) NULL,
	[Id] [uniqueidentifier] NULL,
	[TaskName] [nvarchar](500) NULL,
	[GroupName] [nvarchar](500) NULL,
	[Interval] [nvarchar](50) NULL,
	[ApiUrl] [nvarchar](500) NULL,
	[AuthKey] [nvarchar](500) NULL,
	[AuthValue] [nvarchar](500) NULL,
	[Describe] [nvarchar](max) NULL,
	[RequestType] [nvarchar](50) NULL,
	[LastRunTime] [datetime] NULL,
	[Status] [int] NULL,
	[PostData] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaskOptions_log] PRIMARY KEY CLUSTERED 
(
	[n_log_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskOptions]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskOptions](
	[Id] [uniqueidentifier] NOT NULL,
	[TaskName] [nvarchar](500) NULL,
	[GroupName] [nvarchar](500) NULL,
	[Interval] [nvarchar](50) NULL,
	[ApiUrl] [nvarchar](500) NULL,
	[AuthKey] [nvarchar](500) NULL,
	[AuthValue] [nvarchar](500) NULL,
	[Describe] [nvarchar](max) NULL,
	[RequestType] [nvarchar](50) NULL,
	[LastRunTime] [datetime] NULL,
	[Status] [int] NULL,
	[PostData] [nvarchar](max) NULL,
 CONSTRAINT [PK_TaskOptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'作业名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'TaskName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'分组名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'GroupName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'间隔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'Interval'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调用接口' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'ApiUrl'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'验证key' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'AuthKey'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'验证值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'AuthValue'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'描述说明' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'Describe'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'请求类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'RequestType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后运行时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'LastRunTime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'运行状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Post数据' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TaskOptions', @level2type=N'COLUMN',@level2name=N'PostData'
GO
/****** Object:  StoredProcedure [dbo].[TestProc1]    Script Date: 11/02/2019 14:39:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[TestProc1]
@Name nvarchar(100)=''        --设置默认值
as
begin
	if(@Name!='')
		select * from RF_User where  CHARINDEX(@Name, Name)>0
	else
		select * from RF_User
end
GO
/****** Object:  Default [DF_RF_Test_f24]    Script Date: 11/02/2019 14:39:40 ******/
ALTER TABLE [dbo].[RF_Test] ADD  CONSTRAINT [DF_RF_Test_f24]  DEFAULT (getdate()) FOR [f24]
GO
/****** Object:  Default [DF_RF_SystemField_Id]    Script Date: 11/02/2019 14:39:40 ******/
ALTER TABLE [dbo].[RF_SystemField] ADD  CONSTRAINT [DF_RF_SystemField_Id]  DEFAULT (newid()) FOR [Id]
GO
/****** Object:  Default [DF_RF_SystemField_CreateDate]    Script Date: 11/02/2019 14:39:40 ******/
ALTER TABLE [dbo].[RF_SystemField] ADD  CONSTRAINT [DF_RF_SystemField_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO
/****** Object:  Default [DF_RF_ProgramButton_IsValidateShow]    Script Date: 11/02/2019 14:39:40 ******/
ALTER TABLE [dbo].[RF_ProgramButton] ADD  CONSTRAINT [DF_RF_ProgramButton_IsValidateShow]  DEFAULT ((1)) FOR [IsValidateShow]
GO
/****** Object:  Default [DF_Table_1_ID]    Script Date: 11/02/2019 14:39:40 ******/
ALTER TABLE [dbo].[RF_Log] ADD  CONSTRAINT [DF_Table_1_ID]  DEFAULT (newid()) FOR [Id]
GO
/****** Object:  Default [DF_RF_Log_WriteTime]    Script Date: 11/02/2019 14:39:40 ******/
ALTER TABLE [dbo].[RF_Log] ADD  CONSTRAINT [DF_RF_Log_WriteTime]  DEFAULT (getdate()) FOR [WriteTime]
GO
/****** Object:  Default [DF_Table_1_OTHERTYPE]    Script Date: 11/02/2019 14:39:40 ******/
ALTER TABLE [dbo].[RF_FlowTask] ADD  CONSTRAINT [DF_Table_1_OTHERTYPE]  DEFAULT ((0)) FOR [OtherType]
GO
/****** Object:  Default [DF_RF_AppLibraryButton_IsValidateShow]    Script Date: 11/02/2019 14:39:40 ******/
ALTER TABLE [dbo].[RF_AppLibraryButton] ADD  CONSTRAINT [DF_RF_AppLibraryButton_IsValidateShow]  DEFAULT ((1)) FOR [IsValidateShow]
GO
/****** Object:  Default [DF_TryTest1_d_time]    Script Date: 11/02/2019 14:39:40 ******/
ALTER TABLE [dbo].[TryTest1] ADD  CONSTRAINT [DF_TryTest1_d_time]  DEFAULT (getdate()) FOR [d_time]
GO
