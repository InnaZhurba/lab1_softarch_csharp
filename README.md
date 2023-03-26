# lab1_softarch_csharp
# SoftArch_lab №3
c# implementation of software architecture laboratory work with addded Hazelcast services <br>
Student: Zhurba Inna <br>

## Additional info

Implemented 3 microservices: <br>
- facade: is a comunicational service that control requests to other two services; <br>
- logger: save messages from the clients that ask for it using facade and send saved messages if it is requested; <br>
- messages: for now it has static message on GET request.  <br>
See additional information in Zhurba_lab3.pdf <br>

### Visualisation

![alt text](visualization.mov)
