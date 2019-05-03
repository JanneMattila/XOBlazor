# Misc

## Releases

### Change base href in html files

Inside `index.html` and `offline.html` files there is magical
html content in the head section:

```html
    <base href="/" />
``` 

That needs to be replaced to match the correct path
in GitHub Pages e.g.

```html
    <base href="/XOBlazorApp/" />
``` 

This can be achieved with following PowerShell:

```powershell
$files = Get-ChildItem . -Include "*.html" -Recurse
foreach($file in $files)
{
  (Get-Content $file) | % { $_ -Replace '<base href="/" />', '<base href="/XOBlazorApp/" />' } | Set-Content $file
}  
``` 
