{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-parts.url = "github:hercules-ci/flake-parts";
  };

  outputs = inputs@{ self, flake-parts, nixpkgs }:
    flake-parts.lib.mkFlake { inherit inputs; } {
      systems = [ "x86_64-linux" "aarch64-linux" ];

      perSystem = { system, pkgs, ... }:
        let
          dotnetRuntime = pkgs.dotnet-runtime_8;
        in {
          packages.default = pkgs.buildDotnetModule rec {
            pname = "3mc-game";
            version = "0.1.0";
            src = ./MyApp;

            nugetDeps = ./MyApp/deps.nix;

            dotnet-sdk = pkgs.dotnet-sdk_8;
            dotnet-runtime = dotnetRuntime;
            
            nativeBuildInputs = [ pkgs.makeWrapper ];

            buildPhase = ''
              dotnet build --configuration Release --no-restore
            '';

            installPhase = ''
              mkdir -p $out/bin
              cp -r bin/Release/net8.0/* $out/bin/

              makeWrapper ${dotnetRuntime}/bin/dotnet $out/bin/MyApp \
                --add-flags "exec $out/bin/MyApp.dll" \
                --prefix LD_LIBRARY_PATH : "${pkgs.lib.makeLibraryPath [
                  pkgs.stdenv.cc.cc.lib
                  pkgs.openssl
                ]}"
            '';
          };

          devShells.default = pkgs.mkShell {
            packages = [
              pkgs.dotnet-sdk_8
              pkgs.omnisharp-roslyn
              pkgs.nuget-to-json
            ];
            
            shellHook = ''
              export DOTNET_ROOT=${pkgs.dotnet-sdk_8}
              mkdir -p .nuget-packages
              export NUGET_PACKAGES="$PWD/.nuget-packages"
            '';
          };
        };
    };
}
