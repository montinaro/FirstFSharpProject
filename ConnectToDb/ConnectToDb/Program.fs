namespace MenuPersister

open System
open Newtonsoft.Json

module Program =

    [<EntryPoint>]
    let main argv = 
        printfn "%A" argv

        let divisionId = Int32.Parse(argv.[0])
        let versionId = Int32.Parse(argv.[1])
        
        let getRootItemsList idMenu = db.getRootItems idMenu 
                                            |> Seq.map(fun (itemBase, itemSet) -> MenuDto.createItems itemBase itemSet) 
                                            |> Seq.toList
        let getMenuList idMenuGroup = db.getMenus idMenuGroup 
                                            |> Seq.map(fun row -> MenuDto.createMenu row (getRootItemsList row.IdMenu)) 
                                            |> Seq.toList
        let getGroupsList idVersion = db.getMenuGroups idVersion 
                                            |> Seq.map(fun row -> MenuDto.createMenuGroup row.Code (getMenuList row.IdMenuGroup)) 
                                            |> Seq.toList
        let getVersionsList divisionId version = db.getVersions divisionId version 
                                                    |> Seq.map(fun row -> MenuDto.createMenuVersion row.Description row.Version (getGroupsList row.IdVersion)) 
                                                    |> Seq.toList

        let result = getVersionsList divisionId versionId

        let menuVersionJson = JsonConvert.SerializeObject(result, Formatting.Indented)
                                
        printfn "VersionJson %s" menuVersionJson
        
        0
