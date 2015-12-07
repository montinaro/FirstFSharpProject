namespace ConnectToDb// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open ConnectToDb.ConnectToDatabase
open System
open Newtonsoft.Json

module Program =

    [<EntryPoint>]
    let main argv = 
        printfn "%A" argv

        let divisionId = Int32.Parse(argv.[0])
        
        let getMenuGroups idVersion = ConnectToDatabase.MenuGroupQuery idVersion
        let getVersions divisionId = ConnectToDatabase.MenuVersionQuery divisionId
        let getMenus idMenuGroup = ConnectToDatabase.MenuQuery idMenuGroup
        let getRootItems idMenu = ConnectToDatabase.ItemsRootQuery idMenu
        let getItems idItem = ConnectToDatabase.ItemsQuery idItem
        let getCountries idItem = ConnectToDatabase.CountriesQuery idItem
        let getCountryProperties idItem = ConnectToDatabase.CountryPropertiesQuery idItem
        let getLabels idProperty = ConnectToDatabase.LabelQuery idProperty
        let getMedia idProperty = ConnectToDatabase.MediaQuery idProperty
        
        let createMenu (menu:ConnectToDatabase.dbSchema.ServiceTypes.Menu) items = 
            new Dto.Menu.Menu(menu.Context_Device, menu.Description, items)

        let createCountry (country:ConnectToDatabase.dbSchema.ServiceTypes.Country) =
            new Dto.Menu.Country(country.SortOrder, country.Hidden, country.CountryId.HasValue, country.CountryId)

        let getCountriesList idMenuItemBase = getCountries idMenuItemBase |> Seq.map(fun row -> createCountry row) |> Seq.toList

        let createCountryProperty country itemProperty =
            new Dto.Menu.CountryProperty(country, itemProperty)

        let createLabel (label:ConnectToDatabase.dbSchema.ServiceTypes.MenuLabel) = 
            new Dto.Menu.Label(label.LanguageId, label.Value)

        let createMedia (media:ConnectToDatabase.dbSchema.ServiceTypes.MenuMedia) = 
            new Dto.Menu.Media(media.LanguageId, media.Uri, media.Type)

        let getLabelsList idProperty = getLabels idProperty |> Seq.map(fun row -> createLabel row) |> Seq.toList

        let getMediaList idProperty = getMedia idProperty |> Seq.map(fun row -> createMedia row) |> Seq.toList

        let createProperty (property:ConnectToDatabase.dbSchema.ServiceTypes.MenuItemProperty) = 
            new Dto.Menu.ItemProperty(property.Gender, property.Season, property.Parameters, property.AbsoluteLink, property.AbsoluteLinkTarget, property.Target, property.TargetType, property.Department, property.AgeRange, (getLabelsList property.IdMenuItemProperty), (getMediaList property.IdMenuItemProperty), property.MacroBrand)

        let getCountryProperties idMenuItemBase = getCountryProperties idMenuItemBase |> Seq.map(fun (country, property) -> createCountryProperty (createCountry country) (createProperty property)) |> Seq.toList


        let rec createItems (itemBase:ConnectToDatabase.dbSchema.ServiceTypes.MenuItemBase) (itemSet:ConnectToDatabase.dbSchema.ServiceTypes.MenuItemSet) = 
            match itemSet = null with
            |false -> new Dto.Menu.ItemSet(itemBase.Code, (getItems itemSet.IdMenuItemBase |> Seq.map(fun (itemBaseChild, itemSetChild) -> createItems itemBaseChild itemSetChild) |> Seq.toList), (getCountriesList itemSet.IdMenuItemBase), itemSet.IdSet.Value) :> Dto.Menu.ItemBase
            |_ -> new Dto.Menu.Item((getCountryProperties itemBase.IdMenuItemBase), itemBase.Code, (getItems itemBase.IdMenuItemBase |> Seq.map(fun (itemBaseChild, itemSetChild) -> createItems itemBaseChild itemSetChild) |> Seq.toList)) :> Dto.Menu.ItemBase

        let getRootItemsList idMenu = getRootItems idMenu |> Seq.map(fun (itemBase, itemSet) -> createItems itemBase itemSet) |> Seq.toList

        let getMenuList idMenuGroup = getMenus idMenuGroup |> Seq.map(fun row -> createMenu row (getRootItemsList row.IdMenu)) |> Seq.toList

        let createMenuGroup code menus =
            new Dto.Menu.MenuGroup(code, menus)

        let getGroupsList idVersion = getMenuGroups idVersion |> Seq.map(fun row -> createMenuGroup row.Code (getMenuList row.IdMenuGroup)) |> Seq.toList
         
        let createMenuVersion description version groups =
            new Dto.Menu.MenuVersion(description, version, groups)

        let getVersionsList divisionId = getVersions divisionId |> Seq.map(fun row -> createMenuVersion row.Description row.Version (getGroupsList row.IdVersion)) |> Seq.toList

        let result = getVersionsList divisionId

        let menuVersionJson = JsonConvert.SerializeObject(result, Formatting.Indented)
                                
        printfn "VersionJson %s" menuVersionJson
        
        0
