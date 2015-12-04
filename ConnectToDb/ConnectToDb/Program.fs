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
        let queryMenuGroups idVersion= ConnectToDatabase.MenuGroupQuery idVersion
        let queryMenu idMenuGroup = ConnectToDatabase.MenuQuery idMenuGroup
        let queryMenuItemBase idMenu = ConnectToDatabase.MenuItemBaseQuery idMenu
        let queryMenuItemChildren idMenuItemBase = ConnectToDatabase.MenuItemChildrenQuery idMenuItemBase
        let queryMenuItemSets idMenuItemBase = ConnectToDatabase.MenuItemSetQuery idMenuItemBase
        let queryMenuItem idMenuItemBase = ConnectToDatabase.MenuItemQuery idMenuItemBase
        let queryCountries idMenuItemBase = ConnectToDatabase.CountryQuery idMenuItemBase
        let querySingleItemSet idMenuItemBase = ConnectToDatabase.SingleItemSetQuery idMenuItemBase
        let queryIsItemSet idMenuItemBase = ConnectToDatabase.IsItemSetQuery idMenuItemBase

        let createMenuVersion description version groups =
            new Dto.Menu.MenuVersion(description, version, groups)
        
        let createMenuGroup code menus =
            new Dto.Menu.MenuGroup(code, menus)

        let createMenu device description items =
            new Dto.Menu.Menu(device, description, items)

        let createMenuItemBase code items = 
            new Dto.Menu.ItemBase(code, items)

        let createMenuItem countryProperties code items =
            new Dto.Menu.Item(countryProperties, code, items)

        let createMenuItemSet code items countries setId =
            new Dto.Menu.ItemSet(code, items, countries, setId)

        let createCountry sortOrder hidden isDefault countryId =
            new Dto.Menu.Country(sortOrder, hidden, isDefault, countryId)

      
        //Fake
        let createItemProperty =
            new Dto.Menu.ItemProperty("D", "A", "", "", "", "", "", "dept", "adult", [], [])

         //Fake
        let createCountryProperties =
            let aCountry = createCountry 1 false false 0
            let aProperty = createItemProperty
            new Dto.Menu.CountryProperty (aCountry, aProperty)::[]

        let createCountries idMenuItemBase = queryCountries idMenuItemBase |> Seq.map(fun row -> createCountry row.SortOrder row.Hidden row.CountryId.HasValue row.CountryId.Value) |> Seq.toList

//        let rec create idMenuItemBase =
//          match (queryIsItemSet idMenuItemBase) with
//          |false -> queryMenuItemChildren idMenuItemBase |> Seq.map(fun row -> createMenuItem createCountryProperties row.Code (create row.IdMenuItemBase)) |> Seq.toList
//          |true -> queryMenuItemChildren idMenuItemBase |> Seq.map(fun row -> createMenuItemSet row.Code (createChildItems row.IdMenuItemBase) (createCountries row.IdMenuItemBase) v)

        let rec createChildItems idMenuItemBase = 
            queryMenuItemChildren idMenuItemBase |> Seq.map(fun row -> 
            if(queryIsItemSet row.IdMenuItemBase > 0) 
                then (createMenuItemSet row.Code (createChildItems row.IdMenuItemBase) (createCountries row.IdMenuItemBase) (querySingleItemSet row.IdMenuItemBase) :> Dto.Menu.ItemBase)
            else
                 (createMenuItem createCountryProperties row.Code (createChildItems row.IdMenuItemBase) :> Dto.Menu.ItemBase)
                
                //(createMenuItem createCountryProperties row.Code (createChildItems row.IdMenuItemBase))
            ) |> Seq.toList

        

       

        let createMenuItemSetList idMenuItemBase = queryMenuItemSets idMenuItemBase |> Seq.map(fun row -> createMenuItemSet row.MenuItemBase.Code (createChildItems row.IdMenuItemBase) (createCountries row.IdMenuItemBase) row.IdSet.Value) |> Seq.toList

        let createMenuItemList idMenuItemBase = queryMenuItem idMenuItemBase |> Seq.map(fun row -> createMenuItem createCountryProperties row.MenuItemBase.Code (createChildItems row.IdMenuItemBase)) |> Seq.toList

        let menuItemChildrenList parentId = queryMenuItemChildren parentId |> Seq.map(fun row -> createMenuItemBase row.Code (createChildItems row.IdMenuItemBase)) |> Seq.toList
            
        let menuItemBaseList idMenu = queryMenuItemBase idMenu |> Seq.map(fun row -> createMenuItemBase row.Code (menuItemChildrenList row.IdMenuItemBase)) |> Seq.toList

        let menuList idMenuGroup = queryMenu idMenuGroup |> Seq.map(fun row -> createMenu row.Context_Device row.Description (menuItemBaseList row.IdMenu)) |> Seq.toList
        
        let menuGroupList idVersion = queryMenuGroups idVersion |> Seq.map(fun row -> createMenuGroup row.Code (menuList row.IdMenuGroup)) |> Seq.toList

        let menuVersionList = queryVersion |> Seq.map(fun row -> createMenuVersion row.Description row.Version (menuGroupList row.IdVersion))

        let menuVersionJson = JsonConvert.SerializeObject(menuVersionList, Formatting.Indented)
                                
        printfn "VersionJson %s" menuVersionJson
        //let item = new Dto.Menu.Item(null, null, "ciao")
        0
