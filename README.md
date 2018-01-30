# SyncServerTrigger
An auto-installable Plastic SCM server-side trigger to keep two or more servers synced.

------
How does this work? syncservertrigger.exe uses the `cm` command line utility in your path to create four triggers in your server, all pointing to the same binary:

* `after-checkin`
* `after-mklabel`
* `after-replicationwrite`
* `after-chattvalue`

This way, every time one of the following things happens...:
* Someone creates a new changeset.
* Someone creates a new label.
* Someone replicates changes to the server.
* Someone changes the value of an attribute for a given object.

...the server will automatically fire the trigger, and the trigger will push the new changes from the source server to the server(s) specified in the configuration!

------
## Downloading instructions
You can download the trigger from the [releases section](https://github.com/PlasticSCM/syncservertrigger/releases) of this repository.
If you want to, you can clone it, and building both from Visual Studio, or using `msbuild` (Windows) or `xbuild` (GNU/Linux & macOS). 

------
## Installation instructions
This trigger is compatible with Windows, GNU/Linux and macOS, using the .NET Framework runtime in Windows, and the Mono Runtime in GNU/Linux and macOS.

In the instructions, `localhost:8087` is the source server, and `caelis.home:9095` is the destination server.

### Windows
As simple as:

```cmd
> syncservertrigger.exe install localhost:8087 caelis.home:9095
```

### GNU/Linux
If you installed the Plastic SCM server from the repository, you can use the Mono Runtime we distribute with it to run the trigger:

```bash
$ /opt/plasticscm5/mono/bin/mono syncservertrigger.exe install localhost:8087 caelis.home:9095
```

If you installed the Plastic SCM server using the binaries zip, or you have the Mono runtime installed:

```bash
$ mono syncservertrigger.exe install localhost:8087 caelis.home:9095
```

You will be asked for the location of the Mono Runtime. If the default one is correct, just press Enter.

### macOS
You will need to install the Mono Runtime before executing `syncservertrigger.exe`. It is available [here](http://www.mono-project.com/download/#download-mac).

```bash
$ mono syncservertrigger.exe install localhost:8087 caelis.home:9095
```

### Warning
Bear in mind that is the server the one that executes the trigger, so the trigger will be executed with the same user (and thus, privileges, user home, path, and environment variables) as the server.
That user **must** have the `cm` command line utility in its path, as well as a valid `client.conf` and `profiles.conf` configuration files inside its Plastic SCM configuration directory (`%LOCALAPPDATA%\plastic4` or `$HOME/.plastic4`)

* On **Windows**, if the Plastic SCM server is run as a service, it will be under the Administrator account.
* On **GNU/Linux**, if you installed the server from the repositories, it will run as a service using the `plasticscm` user account, which does not have a home directory.
* On **macOS**, if you installed the server using the PKG installer, it will run as a service using the `plasticscm` user account, which does not have a home directory.

------
## Filtering out repositories
The `syncservertrigger` will push new changes from all of your repositories. This way, if you create new repositories, you don't need to configure the `syncservertrigger` again.
But if you want to opt-out a repository from the synchronization process (for example, if you don't want to push the changes from your `personal` repo), you can do so the following way:

```
> syncservertrigger.exe repofilter add personal
```

You can list the filtered repos:

```
> syncservertrigger.exe repofilter list
```

And remove filters as well:

```
> syncservertrigger.exe repofilter delete personal
```

------
## Mapping source and destination repositories
If your local and remote repository names do not match.
For example, if you want to push the changes on your `codice_local` repo to the `codice_central` destination repository on server `caelis.home:9095`, you can do so the following way:

```
> syncservertrigger.exe repomap add codice_local codice_central caelis.home:9095
```

You can list the configured mappings:

```
> syncservertrigger.exe repomap list
```

And remove mappings as well:

```
> syncservertrigger.exe repomap delete codice_local codice_central caelis.home:9095
```

------
## Adding and removing destination servers
You can add more than one destination server.
For example, you can add both your office's server, and your Plastic Cloud one.

```
> syncservertrigger.exe server add scm.intranet.com:9097
> syncservertrigger.exe server add organization@cloud
```

You can list the destination servers:

```
> syncservertrigger.exe server list
```

And remove them as well:

```
> syncservertrigger.exe server delete scm.intranet.com:9097
```

------
## Getting emails when something went wrong
You can configure an email account to send emails to when the replication process failed.
Right now, only SMTP servers are supported. To start the email account configuration wizard:

```
> syncservertrigger.exe warnemail configure
```

Bear in mind that the password of the account will be stored with a symmetric encryption key.

You can now configure the destination email addresses:

```
> syncservertrigger.exe warnemail add thisemail@address.com
```

List the destination addresses:

```
> syncservertrigger.exe warnemail list
```

And remove them as well:

```
> syncservertrigger.exe warnemail delete thisemail@address.com
```