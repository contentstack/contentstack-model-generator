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
| `-a` | `--api-key` | The Stack API key for the Content Delivery API |
| `-d` | `--delivery-token` | The Delivery token for the Content Delivery API |
| `-h` | `--host` | The Contentstack Host for the Content Delivery API |
| `-n` | `--namespace` | The namespace the classes should be created in |
| `-f` | `--force` | Automatically overwrite files that already exist |
| `-m` | `--modular-block-prefix` | The Modular block Class Prefix. |
| `-g` | `--group-prefix` | The Modular block Class Prefix. |
| `-p` | `--path` | Path to the file or directory to create files in. |

### Example 1
To create classes in current directory run following command:
```
contentstack.model.generator -a <stack_api_key> -d <delivery_token>
```

### Example 2
To create classes in specific path run following command:
```
contentstack.model.generator -a <stack_api_key> -d <delivery_token> -p /User/xxx/Desktop
```

### Example 3
To create classes with namespace run following command:
```
contentstack.model.generator -a <stack_api_key> -d <delivery_token> -n YourProject.Models
```





##
