namespace ConnectToDb.ConnectToDatabase

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

    let MenuItemBaseQuery idMenu =
        query {
            for row in db.MenuItemBase do
            where (row.MenuId = idMenu && row.ParentId.HasValue = false)
            select row
        }
    
    let MenuItemQuery idMenuItemBase =
        query {
            for row in db.MenuItem do
            where (row.IdMenuItemBase = idMenuItemBase)
            select row
        }

    let MenuItemChildrenQuery idMenuItemBase =
        query {
            for row in db.MenuItemBase do 
            where (row.ParentId.Value = idMenuItemBase) 
            select row
        }

    let MenuItemSetQuery idMenuItemBase =
        query {
            for row in db.MenuItemSet do
            where (row.IdMenuItemBase = idMenuItemBase)
            select row
        }

    let CountryQuery idMenuItemBase =
        query {
            for row in db.Country do
            where (row.MenuItemId = idMenuItemBase)
            select row
        }
        