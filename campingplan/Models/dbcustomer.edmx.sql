
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/26/2020 00:11:30
-- Generated from EDMX file: D:\camping plan\campingplan\campingplan\Models\dbcustomer.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [campingplan];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[categorys]', 'U') IS NOT NULL
    DROP TABLE [dbo].[categorys];
GO
IF OBJECT_ID(N'[dbo].[customer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[customer];
GO
IF OBJECT_ID(N'[dbo].[product]', 'U') IS NOT NULL
    DROP TABLE [dbo].[product];
GO
IF OBJECT_ID(N'[dbo].[product_features]', 'U') IS NOT NULL
    DROP TABLE [dbo].[product_features];
GO
IF OBJECT_ID(N'[dbo].[product_features_type]', 'U') IS NOT NULL
    DROP TABLE [dbo].[product_features_type];
GO
IF OBJECT_ID(N'[dbo].[product_typedetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[product_typedetail];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'customer'
CREATE TABLE [dbo].[customer] (
    [rowid] int IDENTITY(1,1) NOT NULL,
    [mno] nvarchar(50)  NULL,
    [maccount] nvarchar(50)  NULL,
    [mpassword] nvarchar(50)  NULL,
    [mname] nvarchar(50)  NULL,
    [mnickname] nvarchar(50)  NULL,
    [memail] nvarchar(250)  NULL,
    [birth_date] datetime  NULL,
    [remark] nvarchar(250)  NULL,
    [varify_code] nvarchar(50)  NULL,
    [isvarify] int  NULL
);
GO

-- Creating table 'categorys'
CREATE TABLE [dbo].[categorys] (
    [rowid] int IDENTITY(1,1) NOT NULL,
    [parentid] int  NULL,
    [category_no] nvarchar(50)  NULL,
    [category_name] nvarchar(250)  NULL
);
GO

-- Creating table 'product'
CREATE TABLE [dbo].[product] (
    [rowid] int IDENTITY(1,1) NOT NULL,
    [categoryid] int  NULL,
    [pno] nvarchar(50)  NOT NULL,
    [plocation] nvarchar(50)  NULL,
    [pname] nvarchar(50)  NULL,
    [pdescription] nvarchar(250)  NULL,
    [psetdate] datetime  NOT NULL,
    [pimg] nvarchar(250)  NULL,
    [pstatus] int  NULL,
    [pmapurl] varchar(max)  NULL
);
GO

-- Creating table 'product_typedetail'
CREATE TABLE [dbo].[product_typedetail] (
    [rowid] int IDENTITY(1,1) NOT NULL,
    [pno] nvarchar(50)  NOT NULL,
    [parea_name] nvarchar(50)  NULL,
    [ptype_no] nvarchar(50)  NULL,
    [ptype_name] nvarchar(50)  NULL,
    [ptype_price] int  NULL,
    [ptype_dep] nvarchar(250)  NULL,
    [productid] int  NOT NULL
);
GO

-- Creating table 'product_features'
CREATE TABLE [dbo].[product_features] (
    [rowid] int IDENTITY(1,1) NOT NULL,
    [pno] nvarchar(50)  NOT NULL,
    [location_type] int  NULL,
    [near_river] int  NULL,
    [near_sea] int  NULL,
    [have_night_view] int  NULL,
    [no_tent] int  NULL,
    [have_canopy] int  NULL,
    [have_clouds] int  NULL,
    [have_firefly] int  NULL,
    [could_book_all] int  NULL,
    [have_rental_equipment] int  NULL,
    [have_game_area] int  NULL,
    [elevation_under_300m] int  NULL,
    [elevation_301m_to_500m] int  NULL,
    [elevation_over_501m] int  NULL
);
GO

-- Creating table 'product_features_type'
CREATE TABLE [dbo].[product_features_type] (
    [rowid] int IDENTITY(1,1) NOT NULL,
    [features_parents_id] int  NULL,
    [features_name] nvarchar(50)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [rowid] in table 'customer'
ALTER TABLE [dbo].[customer]
ADD CONSTRAINT [PK_customer]
    PRIMARY KEY CLUSTERED ([rowid] ASC);
GO

-- Creating primary key on [rowid] in table 'categorys'
ALTER TABLE [dbo].[categorys]
ADD CONSTRAINT [PK_categorys]
    PRIMARY KEY CLUSTERED ([rowid] ASC);
GO

-- Creating primary key on [pno] in table 'product'
ALTER TABLE [dbo].[product]
ADD CONSTRAINT [PK_product]
    PRIMARY KEY CLUSTERED ([pno] ASC);
GO

-- Creating primary key on [rowid] in table 'product_typedetail'
ALTER TABLE [dbo].[product_typedetail]
ADD CONSTRAINT [PK_product_typedetail]
    PRIMARY KEY CLUSTERED ([rowid] ASC);
GO

-- Creating primary key on [pno] in table 'product_features'
ALTER TABLE [dbo].[product_features]
ADD CONSTRAINT [PK_product_features]
    PRIMARY KEY CLUSTERED ([pno] ASC);
GO

-- Creating primary key on [rowid] in table 'product_features_type'
ALTER TABLE [dbo].[product_features_type]
ADD CONSTRAINT [PK_product_features_type]
    PRIMARY KEY CLUSTERED ([rowid] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [pno] in table 'product_typedetail'
ALTER TABLE [dbo].[product_typedetail]
ADD CONSTRAINT [FK_productproduct_typedetail]
    FOREIGN KEY ([pno])
    REFERENCES [dbo].[product]
        ([pno])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_productproduct_typedetail'
CREATE INDEX [IX_FK_productproduct_typedetail]
ON [dbo].[product_typedetail]
    ([pno]);
GO

-- Creating foreign key on [pno] in table 'product_features'
ALTER TABLE [dbo].[product_features]
ADD CONSTRAINT [FK_productproduct_features]
    FOREIGN KEY ([pno])
    REFERENCES [dbo].[product]
        ([pno])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------