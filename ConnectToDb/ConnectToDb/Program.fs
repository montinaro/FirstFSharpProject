namespace ConnectToDb// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open ConnectToDb.ConnectToDatabase
open System
open Newtonsoft.Json

module Program =

    [<EntryPoint>]
    let main argv = 
        printfn "%A" argv

        let division = argv.[0]
        
        let queryVersion = ConnectToDatabase.MenuVersionQuery (Int32.Parse(division))

        queryVersion |> Seq.iter (fun row -> 
                                let menuVersion = {Dto.Menu.VersionDescription = row.Description; Dto.Menu.VersionId = row.IdVersion; Dto.Menu.Groups = []}
                                
                                let groups = ConnectToDatabase.MenuGroupQuery menuVersion.VersionId

                                groups |> Seq.iter (fun row ->
                                    let menuGroup = {Dto.Menu.MenuGroup.Code = row.Code; Dto.Menu.MenuGroup.GroupId = row.IdMenuGroup}
                                    menuVersion.Groups <- menuGroup::menuVersion.Groups
                                    )
                                
                                let menuVersionJson = JsonConvert.SerializeObject(menuVersion, Formatting.Indented)
                                
                                printfn "VersionJson %s" menuVersionJson
                                )
        0
