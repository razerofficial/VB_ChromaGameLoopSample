Imports System

Namespace ChromaSDK
    Module RazerErrors
        REM // Error codes
        REM //! Invalid
        Public Const RZRESULT_INVALID As Integer = -1
        REM //! Success
        Public Const RZRESULT_SUCCESS As Integer = 0
        REM //! Access denied
        Public Const RZRESULT_ACCESS_DENIED As Integer = 5
        REM //! Invalid handle
        Public Const RZRESULT_INVALID_HANDLE As Integer = 6
        REM //! Not supported
        Public Const RZRESULT_NOT_SUPPORTED As Integer = 50
        REM //! Invalid parameter.
        Public Const RZRESULT_INVALID_PARAMETER As Integer = 87
        REM //! The service has Not been started
        Public Const RZRESULT_SERVICE_NOT_ACTIVE As Integer = 1062
        REM //! Cannot start more than one instance of the specified program.
        Public Const RZRESULT_SINGLE_INSTANCE_APP As Integer = 1152
        REM //! Device Not connected
        Public Const RZRESULT_DEVICE_NOT_CONNECTED As Integer = 1167
        REM //! Element Not found.
        Public Const RZRESULT_NOT_FOUND As Integer = 1168
        REM //! Request aborted.
        Public Const RZRESULT_REQUEST_ABORTED As Integer = 1235
        REM //! An attempt was made to perform an initialization operation when initialization has already been completed.
        Public Const RZRESULT_ALREADY_INITIALIZED As Integer = 1247
        REM //! Resource Not available Or disabled
        Public Const RZRESULT_RESOURCE_DISABLED As Integer = 4309
        REM //! Device Not available Or supported
        Public Const RZRESULT_DEVICE_NOT_AVAILABLE As Integer = 4319
        REM //! The group Or resource Is Not in the correct state to perform the requested operation.
        Public Const RZRESULT_NOT_VALID_STATE As Integer = 5023
        REM //! No more items
        Public Const RZRESULT_NO_MORE_ITEMS As Integer = 259
        REM //! DLL Not found
        Public Const RZRESULT_DLL_NOT_FOUND As Integer = 6023
        REM //! Invalid signature
        Public Const RZRESULT_DLL_INVALID_SIGNATURE As Integer = 6033
        REM //! General failure.
        Public Const RZRESULT_FAILED As Integer = &H80004005

        Public Function GetResultString(result As Integer) As String
            Select Case result
                Case RZRESULT_INVALID
                    Return "RZRESULT_INVALID"
                Case RZRESULT_SUCCESS
                    Return "RZRESULT_SUCCESS"
                Case RZRESULT_ACCESS_DENIED
                    Return "RZRESULT_ACCESS_DENIED"
                Case RZRESULT_INVALID_HANDLE
                    Return "RZRESULT_INVALID_HANDLE"
                Case RZRESULT_NOT_SUPPORTED
                    Return "RZRESULT_NOT_SUPPORTED"
                Case RZRESULT_INVALID_PARAMETER
                    Return "RZRESULT_INVALID_PARAMETER"
                Case RZRESULT_SERVICE_NOT_ACTIVE
                    Return "RZRESULT_SERVICE_NOT_ACTIVE"
                Case RZRESULT_SINGLE_INSTANCE_APP
                    Return "RZRESULT_SINGLE_INSTANCE_APP"
                Case RZRESULT_DEVICE_NOT_CONNECTED
                    Return "RZRESULT_DEVICE_NOT_CONNECTED"
                Case RZRESULT_NOT_FOUND
                    Return "RZRESULT_NOT_FOUND"
                Case RZRESULT_REQUEST_ABORTED
                    Return "RZRESULT_REQUEST_ABORTED"
                Case RZRESULT_ALREADY_INITIALIZED
                    Return "RZRESULT_ALREADY_INITIALIZED"
                Case RZRESULT_RESOURCE_DISABLED
                    Return "RZRESULT_RESOURCE_DISABLED"
                Case RZRESULT_DEVICE_NOT_AVAILABLE
                    Return "RZRESULT_DEVICE_NOT_AVAILABLE"
                Case RZRESULT_NOT_VALID_STATE
                    Return "RZRESULT_NOT_VALID_STATE"
                Case RZRESULT_NO_MORE_ITEMS
                    Return "RZRESULT_NO_MORE_ITEMS"
                Case RZRESULT_DLL_NOT_FOUND
                    Return "RZRESULT_DLL_NOT_FOUND"
                Case RZRESULT_DLL_INVALID_SIGNATURE
                    Return "RZRESULT_DLL_INVALID_SIGNATURE"
                Case RZRESULT_FAILED
                    Return "RZRESULT_FAILED"
            End Select
            Return result.ToString()
        End Function
    End Module
End Namespace
