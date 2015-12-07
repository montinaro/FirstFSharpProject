namespace ConnectToDb.Dto

open System

module Menu =
    open Newtonsoft.Json
    
    type Country(sortOrder : int, hidden : bool, isDefault: bool, countryId:Nullable<int>) =
        member this.sortOrder = sortOrder
        member this.hidden = hidden
        member this.isDefault = isDefault
        member this.countryId = countryId

    type Label(languageId : int, value : string)=
        member this.languageId = languageId
        member this.value = value

    type Media(languageId : int, uri : string, typeMedia : string)=
        member this.languageId = languageId
        member this.uri = uri
        member this.typeMedia = typeMedia

    type ItemProperty(gender: string, season : string, parameters : string, absoluteLink : string, absoluteTarget : string, target: string, targetType : string, department : string, ageRange : string, labels : Label list, medias : Media list, macroBrand : Nullable<int>) =
        member this.gender = gender
        member this.season = season
        member this.parameters = parameters
        member this.absoluteLink = absoluteLink
        member this.absoluteTarget = absoluteTarget
        member this.target = target
        member this.targetType = targetType
        member this.department = department
        member this.ageRange = ageRange
        member this.labels = labels
        member this.medias = medias
        member this.macroBrand = macroBrand
        
    type CountryProperty(country:Country, itemProperty:ItemProperty) = 
        member this.country = country
        member this.itemProperty = itemProperty

    type ItemBase(code : string, items : ItemBase list) = 
        member this.Code = code
        member this.Items = items

    type Item(countryProperties : CountryProperty list, code : string, items : ItemBase list) = 
        inherit ItemBase(code, items)
        member this.countryProperties = countryProperties

    type ItemSet(code : string, items : ItemBase list, countries : Country list, setId : int) =
        inherit ItemBase(code, items)
        member this.countries = countries
        member this.setId = setId

    type Menu(device : string, description : string, items : ItemBase list) =
        member this.device = device
        member this.description = description
        member this.items = items
        
    type MenuGroup(code : string, menus : Menu list) = 
        member this.code = code
        member this.menus = menus

    type MenuVersion(versionDescription : string, versionId : int, groups : MenuGroup list) =  
        member this.versionDescription = versionDescription
        member this.versionId = versionId
        member this.groups = groups

