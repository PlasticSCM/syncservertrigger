using System;

namespace syncservertrigger
{
    class Program
    {
        internal enum Commands
        {
            // User commands
            Help,
            Install,
            Uninstall,
            Server,
            RepoFilter,
            RepoMap,
            WarnEmail,

            // Command to be executed by Plastic SCM
            Trigger,

            // Command to be executed by syncservertrigger itself
            Execute
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine(HELP);
                return;
            }

            Commands command = ParseCommand(args[0]);

            if (command == Commands.Help)
            {
                Console.Error.Write(HELP);
                return;
            }
        }

        static Commands ParseCommand(string command)
        {
            if (AreEqual(command, "help") || AreEqual(command, "--help"))
                return Commands.Help;

            if (AreEqual(command, "install"))
                return Commands.Install;

            if (AreEqual(command, "uninstall"))
                return Commands.Uninstall;

            if (AreEqual(command, "server"))
                return Commands.Server;

            if (AreEqual(command, "repofilter"))
                return Commands.RepoFilter;

            if (AreEqual(command, "repomap"))
                return Commands.RepoMap;

            if (AreEqual(command, "warnemail"))
                return Commands.WarnEmail;

            if (AreEqual(command, "trigger"))
                return Commands.Trigger;

            if (AreEqual(command, "execute"))
                return Commands.Execute;

            return Commands.Help;
        }

        static bool AreEqual(string left, string right)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(left, right) == 0;
        }

        const string HELP =
@"Please, keep in mind the following restrictions:

* This executable must be placed in the same machine as the server, as it is
needed to execute the server-side triggers it relies on.

* The 'cm' executable must be installed in the same machine as the server and
available in the PATH environment variable, as it is needed for the replication
process.It must be correctly configured, with the necessary profiles if needed
in order to comunicate with the destination servers.You can read about server
profiles here: https://www.plasticscm.com/documentation/distributed/plastic-scm-version-control-distributed-guide.shtml#Managingremoteauthentication

* The Plastic SCM account configured in client.conf must have the mktrigger
permission granted.

* The Plastic SCM account configured in client.conf must have the rmtrigger
permission granted.


These are the commands supported by syncservertrigger:

install         Autoinstalls the required triggers to have the two servers in
                sync.

Usage: 
    install <src_server> <dst_server>
    
    src_server  The source repository, where the server-side triggers will
                actually be installed.
    dst_server  The destination repository, where the changes will be pushed to.

Examples:
    synservertrigger install localhost:8087 skull:9097
    (Installs the server-side triggers on the 'localhost:8087' server and adds
    'skull:9097' as the first destination repository in the configuration.)

Remarks:
    This command must be executed only for the first destination repository.
    If you want to have multiple destination repositories, check the 'server add'
    command.

    By default, every repository on the source server is expected to exist with
    the same name on the destination server.If that is not the case, check the
    'repomap' command.

    By default, every repository on the source server is synced with the remote
    server.If that is not the desired behavior, check the 'repofilter' command.



uninstall       Autoremoves the triggers from the source server.

Examples:
    syncservertrigger uninstall
    (Uninstalls the server-side triggers, so it won't be automatically synced
    again.)

Remarks:
    This command only needs to be executed once, regardless of the
    number of destination repositories configured using the install command.

    After uninstalling the syncservertrigger, all of the configuration remains
    intact, in case you want to re-install it later.



server          Used to operate on the remote servers synchronization
                configuration.

Usage:
    server <list | <add|delete> dst_server>

    list        Lists the configured destination servers.
    add         Adds 'dst_server' to the list of destination servers.
    delete      Deletes 'dst_server' from the list of destination servers.

Examples:
    syncservertrigger server list
    (Shows a list of the configured destination servers.)

    syncservertrigger server add ssl://diana.mydomain:8088
    (Adds the specified server to the list of destination servers.)

    syncservertrigger server remove ssl://diana.mydomain:8088
    (Removes the specified server from the list of destination servers.)

Remarks:
    By default, every repository on the source server is expected to exist with
    the same name on the destination server.If that is not the case, check the
    'repomap' command.

    By default, every repository on the source server is synced with the remote
    server. If that is not the desired behavior, check the 'repofilter' command.



repofilter      Used to operate on the local repositories synchronization
                configuration.

Usage:
    repofilter <list | <remove|add> repository>


    list        Lists the repositories that will be pushed to the destination
                servers. These are your local repositories, without the ones you
                manually removed.
    remove      Removes a repository from the filter list.
    add         Adds a repository to the filter list.
    repository  The name of the repository on the source server.

Examples:
    syncservertrigger repofilter add codice_local
    (Adds the 'codice_local' repository to the list of filtered-out repositories,
    so it won't be synced with any of the destination servers.)

    syncservertrigger repofilter remove codice_local
    (Removes the 'codice_local' repository from the filtered-out repositories,
    so it will be synced with the destination servers.)

Remarks:
    By default, all of the local repositories are synchronized,
    including new ones.You have to manually add a repository to the filter list
    if you don't want it to be synced. The 'remove' command only applies for
    filtered repositories, to re-admit them in the synchronization process.



repomap         Used to operate on the local repositories mapping with remote
                repositories.

Usage:
    repomap <list | <add|remove> <src_repo> <dst_repo>@<dst_server>>


    list        Lists the repository name mappings.
    add         Adds a new mapping to the list.
    remove      Removes the specified mapping from the list.
    src_repo    The name of the repository in the source server.
    dst_repo    The name of the repostiroy in the destination server.
    dst_server  The destination server, including port.

Examples:
    syncservertrigger repomap add codice_local codice @skull:9097
    (Maps the 'codice_local' source repository to the 'codice' destination
    repository only for the 'skull:9097' destination server.)

    synservertrigger repomap remove codice_local codice @skull:9097
    (Removes the mapping between the source repository 'codice_local' and the
    destination repository 'codice' only for the 'skull:9097' destination server.)

    synservertrigger repomap list
    (Shows a list of the configured mappings.)

Remarks:
    By default, synservertrigger expect your source and destination
    repositories to be named the same.Use this command only if that is not the
    case.

    The 'dst_server' param must match one of the servers in the
    configuration, use the 'server list' command to list the destination servers.



warnemail       Used to configure the warning email notifications when
                synchronization fails.

Usage:
    syncservertrigger warnemail <<configure> | <add|remove> <email>>

    configure   Enters the configuration wizard, to configure the email account
                from which the notifications should be sent.
    add         Adds a destination email address to receive notifications.
    delete      Deletes a destination email address to receive notifications.
    email       Specifies the detination email address to add or remove from the
                notifications recipients.

Examples:
    syncservertrigger configure
    (Enters the configuration wizard.You will be asked for a email address,
    if it should use SMTP or POP3, the email server, and a password.The password
    will be stored cyphered, while the rest of the configuration will be saved
    as plain text. However, the cypher is reversible, so don't use this on a
    machine you don't trust to be secure.)

    synservertrigger add sluisp @somedomain.org
    (Adds the 'sluisp@somedomain.org' to the recipients of warning emails
    regarding synchronization.)

Remarks:
    Emails are only sent when the synchronization process failed. No news is
    good news!";
    }
}
