# SanteDB Daemon Services

In SanteDB, services which implement the IDaemonService interface are treated as system processes that need to be started/stopped on application context. Daemons are useful when:

* You need to have a service register other services on startup
* You want a background task to run for a long period of time
* You want to subscribe to system events or events from other services


Daemon services differ from passive service interfaces in that they are activated by the SanteDB host process explicitly, other services are merely constructed at startup and never notified when the service 
host stops.