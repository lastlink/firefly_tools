# python csv cleaner for firefly III import

* will take a csv and prepare a cleaned up version
* will have an accounts from description that have dates cleaned out
* will separate amounts into debit and credit amounts

## development

* php simple server to test date output
    * `php -S localhost:8000`
* python
    * vm
    ```
    python3 -m venv venv
    source venv/bin/activate
    pip install -r requirements.txt
    ```
    * `python3 bankParser.py`
    * `python3 run.py --ifile input.csv -o wells/output.csv`
    * `pip3 install sqlacodegen`
* dotnet
    * install dep
        * `dotnet add Firefly.Import package McMaster.Extensions.CommandLineUtils`
    * `dotnet run --project Firefly.Import -i Checking1.csv -o wells/output.csv -b wells`
    * `dotnet Firefly.Import\bin\Debug\netcoreapp2.2\Firefly.Import.dll`