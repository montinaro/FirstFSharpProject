namespace MenuPersister
open System

module MenuDto =
    
    type Country(country:db.dbSchema.ServiceTypes.Country) = //(SortOrder : int, Hidden : bool, IsDefault: bool, CountryId:Nullable<int>) =
        member this.SortOrder = country.SortOrder
        member this.Hidden = country.Hidden
        member this.CountryId = country.CountryId

    type Label(label:db.dbSchema.ServiceTypes.MenuLabel) =
        member this.LanguageId = label.LanguageId
        member this.Value = label.Value

    type Media(media:db.dbSchema.ServiceTypes.MenuMedia)=
        member this.LanguageId = media.LanguageId
        member this.Uri = media.Uri
        member this.typeMedia = media.Type

    let getLabelsList idProperty = db.getLabels idProperty 
                                        |> Seq.map(fun row -> new Label(row)) 
                                        |> Seq.toList

    let getMediaList idProperty = db.getMedia idProperty 
                                    |> Seq.map(fun row -> new Media(row)) 
                                    |> Seq.toList

    type ItemProperty(property:db.dbSchema.ServiceTypes.MenuItemProperty) =
        member this.Gender = property.Gender
        member this.Season = property.Season
        member this.Parameters = property.Parameters
        member this.AbsoluteLink = property.AbsoluteLink
        member this.AbsoluteLinkTarget = property.AbsoluteLinkTarget
        member this.Target = property.Target
        member this.TargetType = property.TargetType
        member this.Department = property.Department
        member this.AgeRange = property.AgeRange
        member this.Labels = (getLabelsList property.IdMenuItemProperty)
        member this.Medias =  (getMediaList property.IdMenuItemProperty)
        member this.MacroBrand = property.MacroBrand

    type CountryProperty(Country:db.dbSchema.ServiceTypes.Country, ItemProperty:db.dbSchema.ServiceTypes.MenuItemProperty) = 
        member this.Country = new Country(Country)
        member this.ItemProperty = new ItemProperty(ItemProperty)
    

    type ItemBase(Code:string, Items: ItemBase list) = 
        member this.Code = Code
        member this.Items = Items

    type Item(CountryProperties : CountryProperty list, Code : string, Items : ItemBase list) = 
        inherit ItemBase(Code, Items)
        member this.CountryProperties = CountryProperties

    type ItemSet(Code : string, Items : ItemBase list, Countries : Country list, SetId : int) =
        inherit ItemBase(Code, Items)
        member this.Countries = Countries
        member this.SetId = SetId

    type Menu(Device : string, Description : string, Items : ItemBase list) =
        member this.Device = Device
        member this.Description = Description
        member this.Items = Items
        
    type MenuGroup(Code : string, Menus : Menu list) = 
        member this.Code = Code
        member this.Menus = Menus

    type MenuVersion(VersionDescription : string, VersionId : int, Groups : MenuGroup list) =  
        member this.VersionDescription = VersionDescription
        member this.VersionId = VersionId
        member this.Groups = Groups

    let createMenu (menu:db.dbSchema.ServiceTypes.Menu) items = 
        new Menu(menu.Context_Device, menu.Description, items)

    let createMenuGroup code menus =
        new MenuGroup(code, menus)

    let createMenuVersion description version groups =
        new MenuVersion(description, version, groups)

    let getCountriesList idMenuItemBase = db.getCountries idMenuItemBase 
                                                |> Seq.map(fun row -> new Country(row)) 
                                                |> Seq.toList
    
    let getCountryProperties idMenuItemBase = db.getCountryProperties idMenuItemBase 
                                                |> Seq.map(fun (country, property) -> new CountryProperty(country, property)) 
                                                |> Seq.toList

    let rec createItems (itemBase:db.dbSchema.ServiceTypes.MenuItemBase) (itemSet:db.dbSchema.ServiceTypes.MenuItemSet) = 
            match itemSet = null with
            |false -> new ItemSet(itemBase.Code, 
                                            (db.getItems itemSet.IdMenuItemBase 
                                                |> Seq.map(fun (itemBaseChild, itemSetChild) -> createItems itemBaseChild itemSetChild) 
                                                |> Seq.toList), 
                                            (getCountriesList itemSet.IdMenuItemBase), 
                                            itemSet.IdSet.Value)
                                         :> ItemBase
            |_ -> new Item((getCountryProperties itemBase.IdMenuItemBase), 
                                        itemBase.Code, 
                                        (db.getItems itemBase.IdMenuItemBase 
                                            |> Seq.map(fun (itemBaseChild, itemSetChild) -> createItems itemBaseChild itemSetChild) 
                                            |> Seq.toList)) 
                                        :> ItemBase