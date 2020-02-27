# FileCreatorHelper
Windows Forms App that takes a text file template and creates new versions of that file based on a list of other files' names.

[img]https://cdn.discordapp.com/attachments/671613050205503511/682403265149009975/FileCreator.gif[/img]

# Template
Your template file is the template for how to create the files.
Wherever the program finds {0} in your template it will replace
it with the other selected file's file name without the extension.

# Example:
"This is a template for {0}" if used with a file named "test" would
then become: "This is a template for test"

# Instructions

1. Click on File -> Load Template.
2. Select the template file.
3. A Template will show up on the left side of the program. You can double click if you want to edit the template.
4. Click on File -> Load Files.
5. Select as many other files as you want.
6. Now under the template in the left new files have appeared, you can double click any of them to edit them.
7. When you're ready you can click in Extension -> Set Extension and set an extension for your new files.
8. To export click File -> Export Files
9. Select the folder where the files will be saved.
10. Your new files should be saved.
