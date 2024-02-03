# Changelog

## [2.2.1] -2024-02-03
Fixed.
- EditorNamespace using in TypeField
- EditorNamespace using in SOScriptAPIInfo
- DSToggle not apply style
- Add object to TypeReader name table

## [2.2.0] -2024-01-28
### **<u>IMPORTANT</u>**
This version has reconstructed the way the ScriptingAPI is edited. As a result, the details and invocation of functions related to this API may be affected. Please be mindful of any update requirements.

Also, a part of the folder structure has been changed. It is recommended to delete it first and then reimport it.

Affected API:
- `DocumentBuilderParser.CalXXXTypeName()` move into `TypeReader` as
    - TypeReader.GetName()
    - TypeReader.GetSignature()
    - TypeReader.GetNestedName()
    - TypeReader.GetGenericName()
    - TypeReader.GetAccessLevel()
    - TypeReader.GetPrefix()
- `DocRuntime.FindAllTypesWhere()`
    - TypeReader.FindAllTypesWhere()
- Animation Feture on 
    - DocVisual
    - DocEditVisual
    - DocPage
    - DocPageVisual
    - DocBookVisual

### Delete.
In the upcoming plan, adjustments will be made to the style, animations, and other aspects of the VisualElement. Therefore, the below content will be temporarily removed.
- Default DocStyle except DarkTheme
- Animation part on DocType

### Add.
- ScriptAPI Edit and View
    - ScriptAPI Editor
    - ScriptAPI Window
- DocComponentProperty, allow you to interaction with DocComponent or SerializedProperty in same API call.
- TypeReader, handle feture about System.Type
- ScriptAPIElement, display info base on MemberInfo/System.Type

### Refactored.
- DSTypeField, better search method and view

### Fixed.
- DocFuncDisplay error on wrong format.
- DocEditMatrix error on zero col/row.

### Obsolete.
- DocFuncDisplay

## [2.1.0] -2023-11-05
### This version change some API about DocStyle VisualElement.
### You need to add a Attribute to your CustomDocEditVisual after this version.
Refactored. DocRuntime
Refactored. DocEditorWindow
Refactored. DocRuntime.NewSomething() -> Group of DocVisualElement class
Improve. DocStyle layout
Add. Component version
Add. DocVisual Attribute
Add. DocPageFactory (old DocTemplate)
Add. DocumentEditor quick tutorial

## [2.0.1] -2023-09-01
Improve. DocStyle layout
Fixed. Parser Number/Typename bug
Add. Simple UIElement Extension Docs
Add. UIElement Extension asmdef

## [2.0.0] -2023-08-10
New Version of Document Builder release
old format are no longer supported. If needed, please download from github page

## [1.2.0] -2023-02-26
Refactored. better UI for edit docs
Add. hotkey for edit docs

## [1.1.1] -2023-02-23
Fixed. missing bookroot while first open

## [1.1.0] - 2023-02-21
Fixed. error from different numberFormat
Fixed. missing doc template folder
Fixed. CS0518 cause from invaiid named .asmdef
Add. export docs as markdown with a summary
Add. allow user add MenuItem to open docs window by other book root

## [1.0.0] - 2022-07-11
This is the first release of Document Builder as a built in package.
