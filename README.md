[![Contentstack](https://www.contentstack.com/docs/static/images/contentstack.png)](https://www.contentstack.com/)

# Contentstack model generator
This utility is use to generate models based on ContentTypes in Stack.

## Installation
To install Contenstack model generator run following command:
```
dotnet tool install  -g contentstack.model.generator
```

## How to use
Once you install ```contentstack.model.generator```  run ```--help``` to view available commands.

| Short key | Long Key | Description |
| -- | -- | -- |
| `-a` | `--api-key` | The Stack API key for the Content Management API |
| `-A` | `--authtoken` | The Authtoken for the Content Management API |
| `-e` | `--endpoint` | The Contentstack Host for the Content Management API |
| `-n` | `--namespace` | The namespace the classes should be created in |
| `-f` | `--force` | Automatically overwrite files that already exist |
| `-m` | `--modular-block-prefix` | The Modular block Class Prefix. |
| `-g` | `--group-prefix` | The Group Class Prefix. |
| `-p` | `--path` | Path to the file or directory to create files in. |

### Example 1
To create classes in current directory run following command:
```
contentstack.model.generator -a <stack_api_key> -A <authtoken>
```

### Example 2
To create classes in specific path run following command:
```
contentstack.model.generator -a <stack_api_key> -A <authtoken> -p /User/xxx/Desktop
```

### Example 3
To create classes with namespace run following command:
```
contentstack.model.generator -a <stack_api_key> -A <authtoken> -n YourProject.Models
```

### MIT License

Copyright (c) 2012-2021 Contentstack

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

