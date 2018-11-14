package callAPIService

//go:generate protoc -I. -I$GOPATH\src\github.com\grpc-ecosystem\grpc-gateway\third_party\googleapis --go_out=plugins=grpc:. API_Service.proto
//go:generate protoc -I. -I$GOPATH\src\github.com\grpc-ecosystem\grpc-gateway\third_party\googleapis --grpc-gateway_out=logtostderr=true:. API_Service.proto
//go:generate protoc -I. -I$GOPATH\src\github.com\grpc-ecosystem\grpc-gateway\third_party\googleapis --swagger_out=logtostderr=true:. API_Service.proto
//go:generate protoc --go_out=plugins=grpc:. Check_Material.proto
//go:generate protoc --go_out=plugins=grpc:. Load_Recipe_Drug.proto
//go:generate protoc --go_out=plugins=grpc:. Move_In_Finish_Drug.proto
