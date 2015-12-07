﻿namespace ConnectToDb.ConnectToDatabase

open Microsoft.FSharp.Data.TypeProviders

module ConnectToDatabase =
    type dbSchema = SqlDataConnection<"Data Source=ysql08dev;Initial Catalog=MenuManagementPreview;Integrated Security=true;;connection timeout=45;Application Name=ConnectToBatabase">
    let db = dbSchema.GetDataContext()
    
    let MenuVersionQuery division =
        query {
            for row in db.MenuVersion do
            where (row.DivisionId  = division) 
            select row
        }

    let MenuGroupQuery versionId =
        query {
            for row in db.MenuGroup do
            where (row.DataVersionId = versionId) 
            select row
        }

    let MenuQuery menuGroupId = 
        query {
            for row in db.Menu do
            where (row.MenuGroupId = menuGroupId)
            select row
        }

    let ItemsRootQuery idMenu =
        query {
            for itemBase in db.MenuItemBase do
            where (itemBase.MenuId = idMenu && itemBase.ParentId.HasValue = false)
            leftOuterJoin itemSet in db.MenuItemSet on
                           (itemBase.IdMenuItemBase = itemSet.IdMenuItemBase) into result
            for selection in result do
            select (itemBase, selection)
        }

    let ItemsQuery idMenuBase =
        query {
            for itemBase in db.MenuItemBase do
            where (itemBase.ParentId.Value = idMenuBase)
            leftOuterJoin itemSet in db.MenuItemSet on
                           (itemBase.IdMenuItemBase = itemSet.IdMenuItemBase) into result
            for selection in result do
            select (itemBase, selection)
        }

    let CountriesQuery  idItem =
        query {
            for country in db.Country do
            where (country.MenuItemId = idItem)
            select country
        }

    let CountryPropertiesQuery idItem = 
        query {
            for country in db.Country do
            where (country.MenuItemId = idItem)
            leftOuterJoin itemProperty in db.MenuItemProperty on
                                (country.MenuItemPropertyId.Value = itemProperty.IdMenuItemProperty) into result
            for selection in result do
            select (country, selection)
        }

    let LabelQuery idProperty =
        query {
            for label in db.MenuLabel do
            where (label.MenuItemPropertyId = idProperty)
            select label
        }

    let MediaQuery idProperty =
        query {
            for media in db.MenuMedia do
            where (media.MenuItemPropertyId = idProperty)
            select media
        }