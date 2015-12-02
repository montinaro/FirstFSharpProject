namespace ConnectToDb.Dto

open System

module Menu =

    type MenuGroup = 
        {Code:string; GroupId:Guid}
        override m.ToString() = sprintf "%s %s" m.Code (m.GroupId.ToString())

    type MenuVersion =  
        {VersionDescription:string; VersionId:Guid; mutable Groups:MenuGroup list}
        override m.ToString() = sprintf "MenuVersion: %s %s MenuGroup: %s" m.VersionDescription (m.VersionId.ToString()) (m.Groups.[0].ToString())

