Public Class ReflectionReader
    Public Shared Function Reflect(ByVal Source As String, ByVal objV As Object) As Collections.Generic.IDictionary(Of String, String)
        Dim result As New Collections.Generic.Dictionary(Of String, String)
        Dim abort As Boolean = False
        Dim iV As Integer = 0
        Dim strV As String = ""
        Dim strName As String = ""
        Dim strSource As String() = Source.Split(".")
        While iV < strSource.Length And Not abort
            Dim mInfos As System.Reflection.MemberInfo() = Nothing
            strV = strSource(iV)
            If strV.Contains("*") Then
                'LOOP THROUGH ALL THE FIELDS/METHODS/PROPERTIES AND APPEND TO THE STRING
            Else
                If strName.Length > 0 Then
                    strName &= "."
                End If
                strName &= strV
                mInfos = objV.GetType.GetMember(strV)
                'SINCE NOT ALL USERS WILL GET THE REFLECTION CORRECT, OFFER A LESS OPTIMIZED LOOKUP
                If mInfos Is Nothing OrElse mInfos.Length = 0 Then
                    mInfos = objV.GetType.GetMember(strV, Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public)
                End If
            End If
            abort = True
            If Not mInfos Is Nothing AndAlso mInfos.Length > 0 Then
                Dim mInfo As System.Reflection.MemberInfo = mInfos(0)
                Select Case mInfo.MemberType
                    Case Reflection.MemberTypes.Constructor
                        'DO NOTHING
                    Case Reflection.MemberTypes.Custom
                        'DO NOTHING
                    Case Reflection.MemberTypes.Event
                        'Dim mTobj As System.Reflection.EventInfo
                        'mTobj = objV.GetType.GetEvent(strV)
                        'DO NOTHING
                    Case Reflection.MemberTypes.Field
                        Dim mTobj As System.Reflection.FieldInfo
                        mTobj = objV.GetType.GetField(mInfos(0).Name)
                        objV = mTobj.GetValue(objV)
                        abort = False
                    Case Reflection.MemberTypes.Method
                        abort = False
                        Dim mTobj As System.Reflection.MethodInfo
                        mTobj = objV.GetType.GetMethod(mInfos(0).Name, New Type() {})
                        'Dim param As System.Reflection.ParameterInfo
                        'For Each param In mtObj.GetParameters()
                        '    If param.IsIn Then
                        '        abort = True
                        '    End If
                        'Next
                        If Not abort Then
                            objV = mTobj.Invoke(objV, Nothing)
                        End If
                    Case Reflection.MemberTypes.NestedType
                        'DO NOTHING
                    Case Reflection.MemberTypes.Property
                        abort = False
                        Dim mTobj As System.Reflection.PropertyInfo
                        mTobj = objV.GetType.GetProperty(mInfos(0).Name, New Type() {})

                        If Not abort Then
                            objV = mTobj.GetValue(objV, Nothing)
                        End If
                    Case Reflection.MemberTypes.TypeInfo
                        'DO NOTHING
                End Select
            ElseIf Not strV.StartsWith("*") Then
                'This may reference a key in a dictionary, or something
                For Each pi As System.Reflection.PropertyInfo In objV.GetType().GetProperties()
                    If pi.PropertyType Is GetType(IDictionary) Then
                        Dim dict As IDictionary = pi.GetValue(objV, Nothing)
                        If dict.Contains(strV) Then
                            objV = dict.Item(strV)
                            abort = False
                            Exit For
                        End If
                    ElseIf pi.PropertyType Is GetType(System.Collections.Generic.IDictionary(Of String, Object)) Then
                        Dim dict As System.Collections.Generic.IDictionary(Of String, Object) = pi.GetValue(objV, Nothing)
                        If dict.ContainsKey(strV) Then
                            objV = dict.Item(strV)
                            abort = False
                            Exit For
                        End If
                    End If
                Next
            Else
                abort = True
                Dim bFields As Boolean = True
                Dim bMethods As Boolean = True
                Dim bProperties As Boolean = True
                If strV.ToUpper().Contains("F") OrElse strV.ToUpper().Contains("M") OrElse strV.ToUpper().Contains("P") Then
                    bFields = False
                    bMethods = False
                    bProperties = False
                    If strV.ToUpper().Contains("F") Then
                        bFields = True
                    End If
                    If strV.ToUpper().Contains("M") Then
                        bMethods = True
                    End If
                    If strV.ToUpper().Contains("P") Then
                        bProperties = True
                    End If
                End If
                For Each mi As System.Reflection.MemberInfo In objV.GetType().GetMembers()
                    Try
                        Select Case mi.MemberType
                            Case Reflection.MemberTypes.Constructor
                                'DO NOTHING
                            Case Reflection.MemberTypes.Custom
                                'DO NOTHING
                            Case Reflection.MemberTypes.Event
                                'Dim mTobj As System.Reflection.EventInfo
                                'mTobj = objV.GetType.GetEvent(strV)
                                'DO NOTHING
                            Case Reflection.MemberTypes.Field
                                If bFields Then
                                    result.Add(strName & "." & mi.Name, objV.GetType.GetField(mi.Name).GetValue(objV))
                                End If
                            Case Reflection.MemberTypes.Method
                                If bMethods Then
                                    Dim mTobj As System.Reflection.MethodInfo
                                    mTobj = objV.GetType.GetMethod(mi.Name, New Type() {})
                                    'Dim param As System.Reflection.ParameterInfo
                                    'For Each param In mtObj.GetParameters()
                                    '    If param.IsIn Then
                                    '        abort = True
                                    '    End If
                                    'Next

                                    result.Add(strName & "." & mi.Name, mTobj.Invoke(objV, Nothing).ToString)
                                End If
                            Case Reflection.MemberTypes.NestedType
                                'DO NOTHING
                            Case Reflection.MemberTypes.Property
                                If bProperties Then
                                    Dim mTobj As System.Reflection.PropertyInfo
                                    mTobj = objV.GetType.GetProperty(mi.Name, New Type() {})
                                    result.Add(strName & "." & mi.Name, mTobj.GetValue(objV, Nothing))
                                End If
                            Case Reflection.MemberTypes.TypeInfo
                                'DO NOTHING
                        End Select
                    Catch ex As Exception
                    End Try
                Next
            End If
            iV += 1
        End While
        If Not abort And Not objV Is Nothing Then
            result.Add(Source, r2i.OWS.Framework.Utilities.Utility.CNullStr(objV, ""))
        End If
        Return result
    End Function
End Class
