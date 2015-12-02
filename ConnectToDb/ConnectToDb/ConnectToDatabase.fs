namespace ConnectToDb.ConnectToDatabase

open System
open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

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

