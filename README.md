# KrugerNationalPark Elephant Model

The basic model for the Kruger National Park simulates the behavior of South African elephants in the Savannah over a longer period of time.
Tourism, feeding sites, water points and climatic influences are combined to study the effects of limited or unlimited resources on elephant population development.

## Starting the Model

First navigate to the folder `Models/KrugerNationalParkBox`.

To start the model the attached `config.json` file is required. The following command must be executed:

```bash
dotnet run --sm config.json
```  

All input files are located in the `resources` directory and can be customized according to your needs.

## Building the Simulation Box

To build the model as an **executable box** the included `build.sh` script must be executed:

```bash
sh ./build.sh
```

The script creates a subfolder `KrugerNationalParkBase` and `zip` files, each containing the compiled simulation program per supported operating system. The simulations can then be executed directly:

```bash
cd KrugerNationalParkBase/KrugerNationalParkBase_MACOSX/
./KrugerNationalParkBox --sm config.json
```

Simulation results are produced in the executed directory or against the configuration in the selected `config.json` file.

> **Warning**
> On macOS there may be problems with the verification of the box and additional files with the extension `*.dylib` and `*.dll`. Please execute the following command to make them accessible in your terminal:
> `xattr -d com.apple.quarantine ./KrugerNationalParkBox`