package MqGrpcProject

//go:generate protoc --go_out=plugins=grpc:. MqGrpcProject.proto
//go:generate protoc --go_out=plugins=grpc:. APIMTLSTio.proto
//go:generate protoc --go_out=plugins=grpc:. APISHTEQio.proto
//go:generate protoc --go_out=plugins=grpc:. APLPRDBMio.proto
//go:generate protoc --go_out=plugins=grpc:. APLRSVPRio.proto
//go:generate protoc --go_out=plugins=grpc:. APMEQRSVio.proto
//go:generate protoc --go_out=plugins=grpc:. APCMTLSTio.proto
